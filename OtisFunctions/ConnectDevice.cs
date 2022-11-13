using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OtisFunctions.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OtisFunctions
{
    public static class ConnectDevice
    {

        [FunctionName("ConnectDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "devices/connect")] HttpRequest req,
            ILogger log)

        {
            try
            {
                using var registryManager =
                    RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));

                var body = JsonConvert.DeserializeObject<DeviceRequest>(await new StreamReader(req.Body).ReadToEndAsync());

                var device = await registryManager.GetDeviceAsync(body.DeviceId);
                //If device is null, create new device
                device ??= await registryManager.AddDeviceAsync(new Device(body.DeviceId));

                var conString = $"{Environment.GetEnvironmentVariable("IotHub").Split(";")[0]};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";

                return new OkObjectResult(conString);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult("Unable to connect to device");
            }
        }
    }


}
