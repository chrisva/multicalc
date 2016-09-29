using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using MultiCalc.Models;

namespace MultiCalc.Functions
{
    public interface IQueueService
    {
        Task<BrokeredMessage> RecieveNextMessageAsync();
        Task SendMessageAsync(CalculateProcessMessage calculateProcessMessage);
    }
}