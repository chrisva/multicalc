using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace MultiCalc.Models
{
    public class CalculateProcessMessageTableEntity : TableEntity
    {
        public string CalculateProcessMessageJson { get; set; }
    }
}