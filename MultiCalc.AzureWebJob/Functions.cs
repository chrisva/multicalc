using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using MultiCalc.Functions;
using MultiCalc.Models;
using Newtonsoft.Json;

namespace MultiCalc.AzureWebJob
{
    public class Functions
    {
        private static QueueService _queueService;
        public static bool RunLoop { get; set; } = true;

        // Function triggered by a one time loop schedule every 10 sec. Should be refactored in most scenarios to use QueueTrigger.
        [NoAutomaticTriggerAttribute]
        public static async Task ProcessMethod(TextWriter log)
        {
            _queueService = new QueueService();

            var hubConnection = new HubConnection("https://multicalcweb.azurewebsites.net/signalr", useDefaultUrl: false);
            var multiCalcProxy = hubConnection.CreateHubProxy("MultiCalcHub");
            await hubConnection.Start();

            while (RunLoop)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                log.WriteLine("Scheduled job fired!");

                var cancellationToken = new WebJobsShutdownWatcher().Token;
                cancellationToken.Register(() =>
                {
                    log.Write($"Cancel: The job is stopped because it was cancelled.");
                    RunLoop = false;
                });

                BrokeredMessage message = null;

                try
                {
                    message = await _queueService.RecieveNextMessageAsync();
                    var calcProcessMessage = JsonConvert.DeserializeObject<CalculateProcessMessage>(message.GetBody<string>());

                    if (calcProcessMessage == null)
                    {
                        log.WriteLine("No message in queue. Waiting for next trigger.");
                    }
                    else
                    {
                        log.WriteLine($"Message: { JsonConvert.SerializeObject(calcProcessMessage)}");
                        calcProcessMessage.Status = CalcStatus.Processing;

                        //Doing the actual calculation
                        calcProcessMessage.SumOfMultiples = SumOfMultiples.To(calcProcessMessage.CalcModel.Factors, calcProcessMessage.CalcModel.ParticularNumberMax);
                        calcProcessMessage.Status = CalcStatus.Successfull;

                        try
                        {
                            await multiCalcProxy.Invoke("LastProcessedMessage", JsonConvert.SerializeObject(calcProcessMessage));
                        }
                        catch (Exception ex)
                        {
                            log.Write($"Something went wrong communicating with SignalR. {ex.Message}");
                            await message.AbandonAsync();
                        }

                        await message.CompleteAsync();
                        log.Write("Message set to complete.");
                    }
                }
                catch (Exception ex)
                {
                    log.WriteLine($"Something went wrong processing the next message from queue. Message: {ex.Message}. StackTrace: {ex.StackTrace}");
                    if(message != null)
                        await message.AbandonAsync();
                }

                stopwatch.Stop();
                
                //Adjust time left in 10 second window.
                var jobInterval = TimeSpan.FromSeconds(10);
                var restTime = jobInterval.Subtract(stopwatch.Elapsed);
                if (restTime.TotalMilliseconds > 0)
                {
                    await Task.Delay(restTime, cancellationToken);
                    log.WriteLine($"Rest time in interval on job start: {restTime.TotalMilliseconds}. Totalt time on last job was: {stopwatch.Elapsed.TotalMilliseconds}");
                }
                else
                {
                    log.WriteLine($"Last job took: {stopwatch.Elapsed.TotalMilliseconds}. No need to wait for new trigger.");

                }
            }
        }
    }
}
