using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using DevicesApiFunctions.Models;

namespace DevicesApiFunctions
{
    /*Kommer åt start o stop på device i iotHub via serviceClient*/
    public static class SendDirectMethod
    {
        public static readonly ServiceClient serviceClient =
            ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));
        
        
        [FunctionName("SendDirectMethod")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "directmethod")] HttpRequest req,
            ILogger log)
        {
           
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
           
            //kommer ha id, methodname och payload
            dynamic data = JsonConvert.DeserializeObject<DirectMethodModel>(requestBody);
           
            var method = new CloudToDeviceMethod(data.MethodName);
           
            if(!string.IsNullOrEmpty(data.PayLoad))
                    method.SetPayloadJson(data.PayLoad);
            
            CloudToDeviceMethodResult result=  await serviceClient.InvokeDeviceMethodAsync(data.DeviceId,method);

            if (result.Status != 200)
                return new BadRequestResult();
            else
            return new OkObjectResult(result.GetPayloadAsJson());
        }
    }
}
