using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultiCalc.Models
{
    public class CalculateProcessMessage
    {
        public CalculateModel CalcModel { get; set; }
        public float SumOfMultiples { get; set; }
        public DateTime TimeOfRequest { get; set; }
        public DateTime? TimeOfEnqueue { get; set; }
        public string RequestId { get; set; }
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