using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OtisFunctions.Models;

namespace OtisFunctions
{
    public static class ConnectDevice
    {
       private static readonly string iothub_connectionstring = Environment.GetEnvironmentVariable("IotHub");

       private static readonly RegistryManager _registryManager =
            RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));
        
        [FunctionName("ConnectDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        
        {
            try
            {
                var body = JsonConvert.DeserializeObject<DeviceRequest>(
                    await new StreamReader(req.Body).ReadToEndAsync());
                var device = await _registryManager.GetDeviceAsync(body.DeviceId);

                if (device is null)
                {
                    device = await _registryManager.AddDeviceAsync(new Device(body.DeviceId));
                }

                var connectionString =
                    $"{iothub_connectionstring.Split(";")[0]};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";

                return new OkObjectResult(connectionString);
            }
            catch
            {
                return new BadRequestResult();
            }

        }
    }

   
}
