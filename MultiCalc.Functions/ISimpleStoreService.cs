using System.Collections.Generic;
using System.Threading.Tasks;
using MultiCalc.Models;

namespace MultiCalc.Functions
{
    public interface ISimpleStoreService
    {
        IEnumerable<CalculateProcessMessage> GetAllMessagesByClientId(string clientId);
        void StoreMessage(CalculateProcessMessage calculateProcessMessage);
        void DeleteAllMessagesByClientId(string clientId);
    }
}