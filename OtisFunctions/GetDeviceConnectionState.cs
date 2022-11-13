using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace OtisFunctions
{
    public static class GetDeviceConnectionState
    {
        private static readonly RegistryManager registryManager =
            RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));

        [FunctionName("GetDeviceConnectionState")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "devices/connect")] HttpRequest req,
            ILogger log)
        {
            string deviceId = req.Query["deviceId"];
            var device = await registryManager.GetDeviceAsync(deviceId);
            if (device != null)
                return new OkObjectResult(device.ConnectionState.ToString());

            return new BadRequestResult();
        }
    }
}
