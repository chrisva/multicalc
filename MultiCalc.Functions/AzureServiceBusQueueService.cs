using System;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using MultiCalc.Models;
using Newtonsoft.Json;

namespace MultiCalc.Functions
{
    public class AzureServiceBusQueueService : IQueueService, IDisposable
    {
        private QueueClient _queueClient;
        private string QueueName = "calculations";

        public async Task SendMessageAsync(CalculateProcessMessage calculateProcessMessage)
        {
            Console.WriteLine("Receiving message from Queue.");
            BrokeredMessage message = null;

            if(_queueClient == null)
                _queueClient = QueueClient.Create(QueueName);

                try
                {
                    var messageBody = JsonConvert.SerializeObject(calculateProcessMessage);
                    message = CreateMessage(Guid.NewGuid().ToString("N"), messageBody);
                    await _queueClient.SendAsync(message);
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Console.WriteLine((string) e.Message);
                        throw;
                    }
                    else
                    {
                        HandleTransientErrors(e);
                    }

                   _queueClient.Close();
            }
        }

        public async Task<BrokeredMessage> RecieveNextMessageAsync()
        {
            Console.WriteLine("Receiving message from Queue.");
            BrokeredMessage message = null;

            if (_queueClient == null)
                _queueClient = QueueClient.Create(QueueName);

            try
            {
                message = await _queueClient.ReceiveAsync(TimeSpan.FromSeconds(5));
            }
            catch (MessagingException e)
            {
                if (!e.IsTransient)
                {
                    Console.WriteLine((string)e.Message);
                    throw;
                }
                else
                {
                    HandleTransientErrors(e);
                }

                _queueClient.Close();
            }

            return message;
        }

        private static BrokeredMessage CreateMessage(string messageId, string messageBody)
        {
            BrokeredMessage message = new BrokeredMessage(messageBody) {MessageId = messageId};
            return message;
        }

        private static void HandleTransientErrors(MessagingException e)
        {
            //If transient error/exception, let’s back-off for 2 seconds and retry 
            Console.WriteLine((string) e.Message);
            Console.WriteLine("Will retry sending the message in 2 seconds");
            Thread.Sleep(2000);
        }

        public void Dispose()
        {
            _queueClient.Close();
        }
    }
}