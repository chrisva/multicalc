using Microsoft.Azure.WebJobs;

namespace MultiCalc.AzureWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            JobHostConfiguration config = new JobHostConfiguration();

            // Add Triggers and Binders for Files and Timer Trigger.
            //config.UseTimers();



            var host = new JobHost(config);

            host.CallAsync(typeof(Functions).GetMethod("ProcessMethod"));

            host.RunAndBlock();
        }
    }
}
