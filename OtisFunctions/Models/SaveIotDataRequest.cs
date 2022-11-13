using Newtonsoft.Json;

namespace OtisFunctions.Models;

public class SaveIotDataRequest
{
    [JsonProperty("deviceId")] public string DeviceId { get; set; } = "";
    [JsonProperty("deviceType")] public string DeviceType { get; set; } = "";
    [JsonProperty("deviceName")] public string DeviceName { get; set; } = "";
    [JsonProperty("location")] public string Location { get; set; } = "";
    [JsonProperty("owner")] public string Owner { get; set; } = "";
    [JsonProperty("data")] public dynamic Data { get; set; }
}