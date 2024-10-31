using System.IdentityModel.Tokens.Jwt;
using Azure.Core;
using Azure.Identity;

namespace Ad_Poc;

public static class PostgreUserService
{
    public static async Task<string> GetPostgreSQLAuthUserName(IConfiguration config)
    {
        var azureAppName = config["WEBSITE_SITE_NAME"];
        if (string.IsNullOrEmpty(azureAppName) is false)
            return azureAppName;

        return await GetLocalUserName();
    }

    private static async Task<string> GetLocalUserName()
    {
        var credential = new AzureCliCredential();
        var token = await credential.GetTokenAsync(new TokenRequestContext(["https://management.azure.com/.default"]));

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token.Token) as JwtSecurityToken;
        var email = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "upn")?.Value;
        return email;
    }
}