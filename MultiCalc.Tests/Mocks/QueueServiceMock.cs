using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using MultiCalc.Functions;
using MultiCalc.Models;
using Newtonsoft.Json;

namespace MultiCalc.Tests.Mocks
{
    public class QueueServiceMock : IQueueService
    {
        public async Task<BrokeredMessage> RecieveNextMessageAsync()
        {
            var messageBody = new CalculateProcessMessage
            {
                RequestId = Guid.NewGuid().ToString("N"),
                CalcModel = new CalculateModel
                {
                    Factors = new[] {3, 5},
                    ParticularNumberMax = 10000
                },
                TimeOfRequest = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 1, 0)),
                Status = CalcStatus.Dequeued
            };
            
            var brokeredMessage = new BrokeredMessage(messageBody);
            return brokeredMessage;
        }

        public Task SendMessageAsync(CalculateProcessMessage calculateProcessMessage)
        {
            throw new NotImplementedException();
        }
    }
}
