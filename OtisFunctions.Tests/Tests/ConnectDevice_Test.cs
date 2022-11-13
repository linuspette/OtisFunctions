using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using OtisFunctions.Models;
using System.Text;

namespace OtisFunctions.Tests.Tests;

public class ConnectDevice_Test
{
    [Fact]
    public async Task Test_Connect_Device()
    {
        var deviceRequest = new DeviceRequest
        {
            DeviceId = "xUnitTest"
        };

        var request = new DefaultHttpRequest(new DefaultHttpContext())
        {
            Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceRequest)))
        };



        var result = await ConnectDevice.Run(request, NullLoggerFactory.Instance.CreateLogger("Null logger"));

        Assert.IsType<OkObjectResult>(result);
    }
}