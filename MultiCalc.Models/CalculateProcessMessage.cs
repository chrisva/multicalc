using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace MultiCalc.Models
{
    public class CalculateProcessMessage
    {
        public CalculateModel CalcModel { get; set; }
        public float SumOfMultiples { get; set; }
        public DateTime TimeOfRequest { get; set; }

        /// <summary>
        /// Request ID is a unit of work value set by the first process that creates this object.
        /// It is also used as a RowKey in Table Storage.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// The ClientId is a unique value that identifies the specific client communicating with 
        /// the system. It is in relation to ClientConnectionType. It is also used as 
        /// as a partition key in Table Store.
        /// </summary>
        public string ClientId { get; set; }

        public ClientConnectionType ClientConnectionType { get; set; }

        public DateTime? TimeOfCompletion { get; set; }
        public CalcStatus Status { get; set; }
        public string StatusMessage { get; set; }
    }

    public enum CalcStatus
    {
        Successfull,
        Enqueued,
        Dequeued,
        Processing,
        Error
    }
}