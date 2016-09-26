using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace MultiCalc.Services
{
    public class QueueService
    {
        private static QueueClient queueClient;
        private static string QueueName = "calculations";
        const Int16 maxTrials = 4;

        private static void RecreateQueue()
        {
            NamespaceManager namespaceManager = NamespaceManager.Create();

            Console.WriteLine("\nCreating Queue ‘{0}’…", QueueName);

            // Delete if exists 
            if (namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.DeleteQueue(QueueName);
            }

            namespaceManager.CreateQueue(QueueName);
        }

        public async Task ScheduleProcessingOfCalcQueueAsync()
        {
          /*  var jobId = BackgroundJob.Schedule(
                () => ProcessMultiCalcQueueAsync(),
                TimeSpan.FromSeconds(10));
        */}

        public async Task ProcessMultiCalcQueueAsync()
        {
            await ReceiveMessagesAsync();
        }

        private async Task ReceiveMessagesAsync()
        {
            Console.WriteLine("\nReceiving message from Queue…");
            BrokeredMessage message = null;

            NamespaceManager namespaceManager = NamespaceManager.Create();
            queueClient = QueueClient.Create(QueueName);
            while (true)
            {
                try
                {
                    //receive messages from Queue 
                    message = queueClient.Receive(TimeSpan.FromSeconds(5));
                    if (message != null)
                    {
                        Console.WriteLine($"Message received: Id = {message.MessageId}, Body = {message.GetBody<string>()}");
                        // Further custom message processing could go here… 
                        message.Complete();
                    }
                    else
                    {
                        //no more messages in the queue 
                        break;
                    }
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                    else
                    {
                        HandleTransientErrors(e);
                    }
                }
            }
            queueClient.Close();
        }

        private static BrokeredMessage CreateSampleMessage(string messageId, string messageBody)
        {
            BrokeredMessage message = new BrokeredMessage(messageBody) {MessageId = messageId};
            return message;
        }

        private static void HandleTransientErrors(MessagingException e)
        {
            //If transient error/exception, let’s back-off for 2 seconds and retry 
            Console.WriteLine(e.Message);
            Console.WriteLine("Will retry sending the message in 2 seconds");
            Thread.Sleep(2000);
        }
    }
}