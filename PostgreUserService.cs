using System.IdentityModel.Tokens.Jwt;
using Azure.Core;
using Azure.Identity;

namespace Ad_Poc;

public static class PostgreUserService
{
    public static async Task<string> GetPostgreSqlAuthUserName(IConfiguration config)
    {
        var azureAppName = GetAzureAppName(config);
        if (string.IsNullOrEmpty(azureAppName) is false)
            return azureAppName;
        return await GetLocalUserName();
    }

    private static string? GetAzureAppName(IConfiguration config)
    {
        return config["WEBSITE_SITE_NAME"];
    }
    
    private static async Task<string> GetLocalUserName()
    {
        try
        {
            var credential = new AzureCliCredential();
            var token = await credential.GetTokenAsync(new TokenRequestContext(["https://management.azure.com/.default"]));

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token.Token) as JwtSecurityToken;
            var email = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "upn")?.Value;
            return email;
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync("Erro ao obter usuário local, não esqueça de logar localmente: az login");
            await Console.Error.WriteLineAsync(e.Message);
            throw;
        }
    }
}