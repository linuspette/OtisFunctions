using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Threading.Tasks;

namespace OtisFunctions.Infrastructure;

public static class KeyVaultConnection
{
    private static readonly string KeyVaultUri = "https://linusdev.vault.azure.net/";

    private static readonly SecretClient _secretClient = new SecretClient(new Uri(KeyVaultUri), new DefaultAzureCredential());
    public static async Task<string> GetIotHubSecretAsync(string secretName)
    {
        try
        {
            var secret = await _secretClient.GetSecretAsync(secretName);

            return secret.Value.Value;
        }
        catch { }

        return null!;
    }
}