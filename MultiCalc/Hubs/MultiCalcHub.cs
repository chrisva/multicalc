using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using MultiCalc.Functions;
using MultiCalc.Models;
using Newtonsoft.Json;

namespace MultiCalc.Hubs
{
    public class MultiCalcHub : Hub
    {
        private IQueueService _queueService;

        public MultiCalcHub(IQueueService queueService)
        {
            _queueService = queueService;
        }

        public void AddMultiCalcToQueue(string factors, string particularNumberMax)
        {
            var connectionId = Context.ConnectionId;
            CalculateModel calcModel;

            try
            {
                calcModel = ModelConverter.ConvertStringsToCalculateModel(factors, particularNumberMax);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Number convertion failed. Invalid list of factors or particularNumberMax: {ex.Message}";
                Console.WriteLine(errorMessage);
                Clients.All.multiCalcAddedToQueueFailed(errorMessage);
                return;
            }

            var processMessage = new CalculateProcessMessage
            {
                CalcModel = calcModel,
                Client = new Client
                {
                    ClientId = connectionId,
                    ClientConnectionType = ClientConnectionType.SignalR
                },
                RequestId = Guid.NewGuid().ToString("N"),
                Status = CalcStatus.Enqueued,
                TimeOfRequest = DateTime.UtcNow
            };

            var messageJson = JsonConvert.SerializeObject(processMessage);

            try
            {
                _queueService.SendMessageAsync(processMessage);
                Console.WriteLine($"Enqued message: {messageJson}");
            }
            catch (Exception ex)
            {
                var errorMessage = $"Could not enque message: {messageJson}. Error: {ex}";
                Console.WriteLine(errorMessage);
                Clients.All.multiCalcAddedToQueueFailed(messageJson, errorMessage);
                return;
            }

            Clients.Client(connectionId).notifyMyMultiCalcJobs(messageJson);
        }
        
        /// <summary>
        ///  Reciever for processed messages. Also gives special notication to the origin reciever if still connected.
        /// </summary>
        /// <param name="message">A serialized CalculateProcessMessage.</param>
        public void LastProcessedMessage(string message)
        {
            var calculateProcessMessage = JsonConvert.DeserializeObject<CalculateProcessMessage>(message);

            Clients.All.notifyAllMessage(message);
            Clients.Client(calculateProcessMessage.Client.ClientId).notifyMyMultiCalcCompleted(message);
        }
    }
}