using System;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Transports;
using MultiCalc.Functions;
using MultiCalc.Models;
using Newtonsoft.Json;

namespace MultiCalc.Hubs
{
    public class MultiCalcHub : Hub
    {
        private readonly IQueueService _queueService;
        private readonly ISimpleStoreService _simpleStoreService;

        public MultiCalcHub(IQueueService queueService, ISimpleStoreService simpleStoreService)
        {
            _queueService = queueService;
            _simpleStoreService = simpleStoreService;
        }

        /// <summary>
        /// Init connection and pass and existing client id to recover.
        /// </summary>
        /// <param name="existingClientId"></param>
        public void InitConnection(string existingClientId)
        {
            var currentId = Context.ConnectionId;
            Clients.Client(currentId).initComplete(currentId);
        }

        /// <summary>
        /// Can be restore the current session with data from another session that never got the results back. Deletes
        /// messages from persistent store afterwards.
        /// </summary>
        /// <param name="existingClientId">The client id to check for persisted data that was never sent.</param>
        public void RestoreSession(string existingClientId)
        {
            var currentId = Context.ConnectionId;

            try
            {
                var messages = _simpleStoreService.GetAllMessagesByClientId(existingClientId);
                foreach (var message in messages)
                {
                    Clients.Client(currentId).notifyMyMultiCalcCompleted(JsonConvert.SerializeObject(message));
                    _simpleStoreService.DeleteAllMessagesByClientId(existingClientId);
                }
            }
            catch (Exception ex)
            {
                Clients.Client(currentId).notifyMyMultiCalcCompleted($"Error processing message: {ex.Message}");
            }
        }

        /// <summary>
        /// Add multi calc to queue for later processing.
        /// </summary>
        /// <param name="factors">The factors.</param>
        /// <param name="particularNumberMax">The particular number max in the range of numbers.</param>
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
                var errorMessage =
                    $"Number convertion failed. Invalid list of factors or particularNumberMax: {ex.Message}";
                Console.WriteLine(errorMessage);
                Clients.All.multiCalcAddedToQueueFailed(errorMessage);
                return;
            }

            var processMessage = new CalculateProcessMessage
            {
                CalcModel = calcModel,
                ClientId = connectionId,
                ClientConnectionType = ClientConnectionType.SignalR,
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

            calculateProcessMessage.Status = CalcStatus.Successfull;
            calculateProcessMessage.TimeOfCompletion = DateTime.UtcNow;

            //Check if client is still connected
            var heartBeat = GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>();

            var connectionAlive = heartBeat.GetConnections().FirstOrDefault(c => c.ConnectionId == calculateProcessMessage.ClientId);

            if (connectionAlive != null && connectionAlive.IsAlive)
            {
                Clients.Client(calculateProcessMessage.ClientId).notifyMyMultiCalcCompleted(JsonConvert.SerializeObject(calculateProcessMessage));
            }
            else
            {
                //Store message to persistent storage since connection is not alive.
                try
                {
                    _simpleStoreService.StoreMessage(calculateProcessMessage);
                }
                catch (Exception ex)
                {
                    Clients.All.notifyAllMessage($"Error processing message: {ex.Message}");
                }
            }

            Clients.All.notifyAllMessage(message);
        }
    }
}