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

namespace DevicesApiFunctions
{
    /*
     Behöver få in properties reported*/
    public static class CreateDevice
    {
        public static readonly RegistryManager registry = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));

        [FunctionName("CreateDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var deviceId = Guid.NewGuid().ToString();
            var device = await registry.AddDeviceAsync(new Device(deviceId));

            var twin = await registry.GetTwinAsync(deviceId);
            string patch = JsonConvert.SerializeObject(new { allowSending = false });

            await registry.UpdateTwinAsync(deviceId, patch, twin.ETag);
            return new OkObjectResult(device);
        }
    }
}
