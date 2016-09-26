using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using MultiCalc.Services;
using Owin;

[assembly: OwinStartup(typeof(MultiCalc.Startup))]

namespace MultiCalc
{
    public partial class Startup
    {
        public static QueueService queueService;

        public async void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            //Start background work
           // queueService = new QueueService();
           // await queueService.ScheduleProcessingOfCalcQueueAsync();
        }
    }
}
