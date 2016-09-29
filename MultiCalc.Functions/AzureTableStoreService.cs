using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MultiCalc.Models;
using Newtonsoft.Json;

namespace MultiCalc.Functions
{
    public class AzureTableStoreService : ISimpleStoreService
    {
        private CloudTable _multiCalcsTable;

        public AzureTableStoreService()
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            _multiCalcsTable = tableClient.GetTableReference("multicalcs");

            // Create the table if it doesn't exist.
            _multiCalcsTable.CreateIfNotExists();
        }

        public IEnumerable<CalculateProcessMessage> GetAllMessagesByClientId(string clientId)
        {
            TableQuery<CalculateProcessMessageTableEntity> query = new TableQuery<CalculateProcessMessageTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, clientId));

            var result = _multiCalcsTable.ExecuteQuery(query);

            List<CalculateProcessMessage> messages = new List<CalculateProcessMessage>();
            foreach (var calculateProcessMessageTableEntity in result)
            {
                messages.Add(JsonConvert.DeserializeObject<CalculateProcessMessage>(calculateProcessMessageTableEntity.CalculateProcessMessageJson));
            }

            // Execute the insert operation.
            return messages;
        }

        public void DeleteAllMessagesByClientId(string clientId)
        {
            TableQuery<CalculateProcessMessageTableEntity> query = new TableQuery<CalculateProcessMessageTableEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, clientId));

            var result = _multiCalcsTable.ExecuteQuery(query);

            foreach (var calculateProcessMessageTableEntity in result)
            {
                var deleteOperation = TableOperation.Delete(calculateProcessMessageTableEntity);
                _multiCalcsTable.Execute(deleteOperation);
            }
        }

        public void StoreMessage(CalculateProcessMessage calculateProcessMessage)
        {
            var tableEntity = new CalculateProcessMessageTableEntity
                {
                    PartitionKey = calculateProcessMessage.ClientId,
                    RowKey = calculateProcessMessage.RequestId,
                    CalculateProcessMessageJson = JsonConvert.SerializeObject(calculateProcessMessage)
                };

            // Create the TableOperation object that inserts the customer entity.
            var insertOperation = TableOperation.Insert(tableEntity);

            // Execute the insert operation.
            _multiCalcsTable.Execute(insertOperation);
        }
    }
}
