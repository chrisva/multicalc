using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Http;
using MultiCalc.Functions;
using MultiCalc.Models;
using Swashbuckle.Swagger.Annotations;

namespace MultiCalc.Worker.Controllers
{
    public class SumOfMultipliesController : ApiController
    {
        private static List<MultiCalcMessageTimer> _activeTimers = new List<MultiCalcMessageTimer>();

        // GET api/values/5
        [SwaggerOperation("ProcessNextMultiCalc")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [HttpPost]
        public HttpResponseMessage ProcessNextMultiCalc()
        {
            //return "value";
            //Starting a new job creating a job every 10 second until
            ScheduleMessageTimer(6, 10);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Running jobs a number of times with interval.
        /// </summary>
        /// <param name="maxIntervals">Defines the number of times to get messages.</param>
        /// <param name="intervalInMilliseconds">Defines the interval in seconds to trigger a new get of message.</param>
        /// <returns></returns>
        public async Task ScheduleMessageTimer(int maxIntervals, int intervalInMilliseconds)
        {
            var messageTimer = new MultiCalcMessageTimer(new QueueService())
            {
                MaxIntervals = maxIntervals,
                Interval = intervalInMilliseconds
            };
            messageTimer.Start();
        }




        // POST api/values
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public void Post([FromBody]string value)
        {
        }
    }
}
