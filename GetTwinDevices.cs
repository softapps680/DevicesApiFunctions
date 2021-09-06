using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DevicesApiFunctions.Models;
using Microsoft.Azure.Devices;
using System.Net.Http;
using System.Collections.Generic;

namespace DevicesApiFunctions
{
    public static class GetTwinDevices
    {
        public static readonly RegistryManager registry = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));
        private static HttpClient client = new HttpClient();

        [FunctionName("GetTwinDevices")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "devices")] HttpRequest req,
            ILogger log)
        {
            var devices = new List<IOTDevice>();
            //kombinera info från db och twin till ettt objekt
            var result = registry.CreateQuery("SELECT * FROM devices");
            var twins = await result.GetNextAsTwinAsync();
            foreach (var twin in twins)
            {
                //unika enheter anropas med api adressen http://localhost:7071/api/devices/ + id så körs getdevicebyid o returnerar
                var url = Environment.GetEnvironmentVariable("GetDeviceByIdUrl") + twin.DeviceId;
                var response = await client.GetAsync(url);
                var data = JsonConvert.DeserializeObject<IOTDevice>(await response.Content.ReadAsStringAsync());
                //
                
                devices.Add(new IOTDevice
                {
                    DeviceId = twin.DeviceId,
                    DeviceType = data.DeviceType,
                    ConnectionState = (twin.ConnectionState.ToString() == "Connected") ? "Online" : "Offline",
                    Status = twin.Status.ToString(),
                    JsonData = data.JsonData,
                    JsonDataLastUpdated = data.JsonDataLastUpdated,
                    AllowSending = twin.Properties.Reported["allowSending"]
                  
                }); 
            }


            return new OkObjectResult(devices);
        }
    }
}
