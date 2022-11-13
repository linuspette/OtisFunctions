using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OtisFunctions.Infrastructure;
using OtisFunctions.Models;
using System.IO;
using System.Threading.Tasks;

namespace OtisFunctions
{
    public static class ConnectDevice
    {

        [FunctionName("ConnectDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)

        {
            var iothub_connectionstring = await KeyVaultConnection.GetIotHubSecretAsync("IotHub");

            var _registryManager =
                RegistryManager.CreateFromConnectionString(iothub_connectionstring);


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
