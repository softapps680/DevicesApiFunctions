using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using DevicesApiFunctions.Models;
using System.Collections.Generic;

namespace DevicesApiFunctions
{
    public static class SaveData
    {
        [FunctionName("SaveData")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "IotHubEndpoint")] EventData message,
             [CosmosDB(
                databaseName:"IOT",
                collectionName: "IotDevicesInfo",
                CreateIfNotExists = true,
                ConnectionStringSetting = "CosmosDB"
            )] out dynamic cosmos,
             ILogger log)
        {

            try
            {
                cosmos = new IOTDevice
                {
                    DeviceId = message.SystemProperties["iothub-connection-device-id"].ToString(),
                    DeviceType = message.Properties["deviceType"].ToString(),
                    JsonData = Encoding.UTF8.GetString(message.Body.Array)
                };
            }
            catch
            {
                cosmos = null;
            }
        }
        //Inkommande messages outputas till cosmosdb
        /* [FunctionName("SaveData")]
         public static void Run(
             [IoTHubTrigger("messages/events", Connection = "IotHubEndpoint", ConsumerGroup = "cosmos")] EventData message,
              [CosmosDB(databaseName: "IoT", collectionName: "devicesinfo", CreateIfNotExists = true, ConnectionStringSetting = "CosmosDB")] out dynamic cosmos,
             ILogger log)
         {
             try { 
             //meddelanden in till iothubben triggar outpu till cosmosdb, nu skapa vad som ska outputtas dit
             //en kan hämta info från meddelandet eller devicetwin
             cosmos = new IOTDevice
             {
                 //iothub system properties==meddelandeformat docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-construct
                 DeviceId = message.SystemProperties["iothub-connection-device-id"].ToString(),
                 DeviceType = message.SystemProperties["deviceType"].ToString(),
                 //meddelandebodyn där finns all info?
                 JsonData = Encoding.UTF8.GetString(message.Body.Array)
             };
             }
             catch { cosmos = null; }
         }*/
    }
}