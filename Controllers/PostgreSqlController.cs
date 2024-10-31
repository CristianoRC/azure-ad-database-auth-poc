using Azure.Core;
using Azure.Identity;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Ad_Poc.Controllers;

[ApiController]
[Route("/postgresql")]
public class PostgreSqlController(IConfiguration configuration) : ControllerBase
{
    private readonly string _connectionString = configuration["postgre:database"]!;
    private readonly string? _entraIdLocalUser = configuration["postgre:entraIdUser"];
    private readonly string? _azureServiceName = configuration["WEBSITE_SITE_NAME"];
    private readonly string _azurePostgreTokenScope = configuration["postgre:tokenScope"]!;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> PostgreSql()
    {
        try
        {
            NpgsqlConnectionStringBuilder connectionStringBuilder = new(_connectionString);
            var token = await GetEntraIdToken();
            connectionStringBuilder.Username = _entraIdLocalUser ?? _azureServiceName;
            connectionStringBuilder.Password = token;

            await using var conn = new NpgsqlConnection(connectionStringBuilder.ToString());
            await conn.OpenAsync();
            const string query = "SELECT schemaname, tablename FROM pg_catalog.pg_tables LIMIT 5;";
            var response = await conn.QueryAsync<dynamic>(query);
            await conn.CloseAsync();
            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, StackTrack = e.StackTrace });
        }
    }
    
    private async Task<string> GetEntraIdToken()
    {
        try
        {
            var azureCredential = new DefaultAzureCredential();
            var tokenScope = new TokenRequestContext([_azurePostgreTokenScope]);
            var tokenResponse = await azureCredential.GetTokenAsync(tokenScope);
            return tokenResponse.Token;
        }
        catch (Exception error)
        {
            var errorMessage = error.InnerException != null ? error.InnerException.Message : "Failed to obtain token";
            await Console.Out.WriteLineAsync($"{error.Message}\n\n{errorMessage}");
            throw;
        }
    }
}