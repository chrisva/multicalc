using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MultiCalc.Functions;
using MultiCalc.Models;
using Newtonsoft.Json;

namespace MultiCalc.Controllers
{
    [RoutePrefix("api/sumofmultiplies")]
    public class SumOfMultipliesController : ApiController
    {
        [Route("calculatesum")]
        [HttpPost]  
        public string PostCalculateSum(CalculateModel model)
        {
            return SumOfMultiples.To(model.Factors, model.ParticularNumberMax).ToString();
        }

        [Route("nextsamplemessage")]
        [HttpPost]
        public CalculateProcessMessage GetSampleMessage()
        {
            return new CalculateProcessMessage
            {
                RequestId = Guid.NewGuid().ToString("N"),
                CalcModel = new CalculateModel
                {
                    Factors = new[] {3,5},
                    ParticularNumberMax = 10000
                },
                TimeOfRequest = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 1, 0)),
                Status = CalcStatus.Dequeued
            };
        }
    }
}
