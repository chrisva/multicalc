using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MultiCalc.Functions;
using MultiCalc.Models;

namespace MultiCalc.Controllers
{
    [RoutePrefix("api/SumOfMultiplies")]
    public class SumOfMultipliesController : ApiController
    {
        [Route("CalculateSum")]
        [HttpPost]  
        public string PostCalculateSum(CalculateModel model)
        {
            return SumOfMultiples.To(model.Factors, model.ParticularNumberMax).ToString();
        }

        [Route("NextMessage")]
        [HttpGet]
        public string GetMessage()
        {
            return $"This is my next message: {DateTime.UtcNow}";
        }

        [Route("NextSampleMessage")]
        [HttpGet]
        public CalculateProcessMessage GetSampleMessage()
        {
            return new CalculateProcessMessage
            {
                CalcModel = new CalculateModel
                {
                    Factors = new[] {3,5},
                    ParticularNumberMax = 10000
                },
                TimeOfRequest = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 1, 0)),
                TimeOfEnqueue = DateTime.UtcNow,
                Status = CalcStatus.Dequeued
            };
        }

        [Route("NotifyResult")]
        [HttpPost]
        public HttpResponseMessage PostNotifyResult(CalculateProcessMessage calcProcessMessage)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent("Message recieved and subscribers are notified.");
            return response;
        }
    }
}
