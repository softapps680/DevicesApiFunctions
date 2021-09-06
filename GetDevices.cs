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
using System.Collections.Generic;
using DevicesApiFunctions.Models;
using System.Linq;

namespace DevicesApiFunctions
{
    public static class GetDevices
    {
        //denna sköter CRUD på devices
        private static readonly RegistryManager registry =
            RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));
        //Lägger connstringen i jsonfilen
        [FunctionName("GetDevices")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(databaseName:"IOT",collectionName: "IotDevicesInfo", CreateIfNotExists = true, ConnectionStringSetting = "CosmosDB", SqlQuery = "SELECT * from c")] IEnumerable<IOTDevice> cosmos
             , ILogger log
            )
        {
            // hämtar från iothubben
            var results = registry.CreateQuery("SELECT * FROM devices");
            //info om devicen
            var twins = await results.GetNextAsTwinAsync();
           
            foreach (var device in cosmos) {
              
              //hämta upp motsvarande twindevice fr iothubben
                var twinDevice = twins.Where(x => x.DeviceId == device.DeviceId).FirstOrDefault();
                device.ConnectionState = (twinDevice.ConnectionState.ToString() == "Connected") ? "Online" : "OffLine";
                device.Status = twinDevice.Status.ToString();
            }

               

                return new OkObjectResult(cosmos);
        }



       
    }
}
