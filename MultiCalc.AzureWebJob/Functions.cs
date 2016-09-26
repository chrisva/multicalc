using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using MultiCalc.Functions;
using MultiCalc.Models;
using Newtonsoft.Json;

namespace MultiCalc.AzureWebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue. Would usually use this to trigger the queue.
        /*   public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
           {
           }*/

        // Function triggered by a timespan schedule every 10 sec.
        public static async Task TimerJob([TimerTrigger("00:00:10")] TimerInfo timerInfo, TextWriter log)
        {
            log.WriteLine("Scheduled job fired!");
            await GetNextMessage(log);
        }

        private static async Task GetNextMessage(TextWriter log)
        {
            var client =  new HttpClient();
            var message = await client.GetStringAsync("http://multicalcweb.azurewebsites.net/api/SumOfMultiplies/NextSampleMessage");

            log.WriteLine("Processing: " + message);

            var calcProcessMessage = JsonConvert.DeserializeObject<CalculateProcessMessage>(message);

            calcProcessMessage.Status = CalcStatus.Processing;
            calcProcessMessage.SumOfMultiples = SumOfMultiples.To(calcProcessMessage.CalcModel.Factors, calcProcessMessage.CalcModel.ParticularNumberMax);
            calcProcessMessage.Status = CalcStatus.Successfull;

            log.WriteLine("Result: " + calcProcessMessage.SumOfMultiples);

            var notifyResult = await client.PostAsync("http://multicalcweb.azurewebsites.net/api/SumOfMultiplies/NotifyResult", new StringContent(JsonConvert.SerializeObject(calcProcessMessage)));

            var testString = await notifyResult.Content.ReadAsStringAsync();

            log.WriteLine(testString);
        }
    }
}
