using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Device2
{
   // ssss
    class Program
    {
        private static readonly DeviceClient deviceClient =
          DeviceClient.CreateFromConnectionString("<connstring på device>", TransportType.Mqtt);
        public static bool allowSending = true;


        private static async Task UpdateReportedSendingPropsAsync()
        {
            //hämta tvillingen du kan plocka desired eller reported properties för uppdatering och läsning antar jag
            var twin = await deviceClient.GetTwinAsync();
            string patch = JsonConvert.SerializeObject(new { allowSending = allowSending });
            //rapportera läget på boolen allowSending
            await deviceClient.UpdateReportedPropertiesAsync(JsonConvert.DeserializeObject<TwinCollection>(patch));
        }
       
        
        private static async Task<MethodResponse> Start(MethodRequest methodRequest, object UserContext)
        {
            //kan få ut info methodrequestobjektet ex
            // methodRequest.DataAsJson; själva payloaden
            allowSending = true;
            //uppdatera twin om läget
            await UpdateReportedSendingPropsAsync();
            Console.WriteLine("Start sending messages!!!");
            return new MethodResponse(new byte[0], 200);
        }
        private static async Task<MethodResponse> Stop(MethodRequest methodRequest, object UserContext)
        {
            allowSending = false;
            //uppdatera twin om läget
            await UpdateReportedSendingPropsAsync();
            Console.WriteLine("Stop sending messages!!!");
            return new MethodResponse(new byte[0], 200);
        }
        static async Task Main(string[] args)
        {
           //om den sänder ska kollas av med en directmethod (trigger, metodnamn,null) metoderna finns i iothubben
            //skapar metoder här för att kunna anropa dem
            await deviceClient.SetMethodHandlerAsync("start", Start, null);
            await deviceClient.SetMethodHandlerAsync("stop", Stop, null);
            //rapportera startläget?
           await deviceClient.UpdateReportedPropertiesAsync(JsonConvert.DeserializeObject<TwinCollection>(JsonConvert.SerializeObject(new { allowSending = allowSending })));
            
            while (true)
            {
                
                while (allowSending)
                {
                    var data = new { temperature = 39, humidity = 49 };
                    var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
                    message.Properties["deviceType"] = "Test2";
                    await deviceClient.SendEventAsync(message);
                    Console.WriteLine($"Message sent {data}");
                    await Task.Delay(10000);
                }
                //kör inte för hårt
                await Task.Delay(500);
            }
        }

       
    }
}
