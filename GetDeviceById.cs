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
using System.Collections.Generic;
using System.Linq;

namespace DevicesApiFunctions
{
    public static class GetDeviceById
    {
        [FunctionName("GetDeviceById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "devices/{id}")] HttpRequest req,
            [CosmosDB(
                databaseName:"IOT",
                collectionName: "IotDevicesInfo",
                CreateIfNotExists = true,
                ConnectionStringSetting = "CosmosDB",
                SqlQuery = "select top 1 * from c where c.DeviceId = {id} order by c._ts desc"
            )] IEnumerable<IOTDevice> devices,
            ILogger log)
        {
            return new OkObjectResult(devices.FirstOrDefault());
        }
    }
}
