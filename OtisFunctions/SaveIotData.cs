using Azure.Messaging.EventHubs;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OtisFunctions.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

namespace OtisFunctions
{
    public class SaveIotData
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("SaveIotData")]
        public void Run([IoTHubTrigger("messages/events", Connection = "IotHubEndpoint")] EventData message,
            [CosmosDB(databaseName: "LpSmartDevices", collectionName: "data", ConnectionStringSetting = "CosmosDB", CreateIfNotExists = true)] out dynamic output, ILogger log)
        {
            try
            {
                using var registryManager =
                    RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));
                var twin = Task.Run(() => registryManager.GetTwinAsync(message.SystemProperties["iothub-connection-device-id"].ToString())).Result;

                var data = new SaveIotDataRequest();

                try { data.DeviceId = message.SystemProperties["iothub-connection-device-id"].ToString() ?? twin.DeviceId; }
                catch { }

                try { data.DeviceType = message.Properties["deviceType"].ToString() ?? twin.Properties.Reported["deviceType"]; }
                catch { }

                try { data.DeviceName = message.Properties["deviceName"].ToString() ?? twin.Properties.Reported["deviceName"]; }
                catch { }

                try { data.Location = message.Properties["location"].ToString() ?? twin.Properties.Reported["location"]; }
                catch { }

                try { data.Owner = message.Properties["owner"].ToString() ?? twin.Properties.Reported["owner"]; }
                catch { }

                try { data.Data = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(message.Body.ToArray())); }
                catch { }

                output = data;
            }
            catch
            {
                output = null;
            }
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.ToArray())}");
        }
    }
}