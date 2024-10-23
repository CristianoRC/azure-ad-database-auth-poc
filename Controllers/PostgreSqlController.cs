using Azure.Core;
using Azure.Identity;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Ad_Poc.Controllers;

[ApiController]
[Route("/postgresql")]
public class PostgreSqlController :ControllerBase
{
    private readonly string _connectionString;
    private readonly string _entraIdUser;
    public PostgreSqlController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("pg-database")!;
        _entraIdUser = configuration["entraIdUser"]!;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> PostgreSql()
    {
        try
        {
            NpgsqlConnectionStringBuilder connectionStringBuilder = new(_connectionString);
            var token = await GetEntraIdPassword();
            connectionStringBuilder.Username = _entraIdUser;
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


    private static async Task<string> GetEntraIdPassword()
    {
        try
        {
            //TODO: Refatorar e dar bons nomes
            var credential = new DefaultAzureCredential();
            var context = new TokenRequestContext(["https://ossrdbms-aad.database.windows.net/.default"]);
            var tokenResponse = await credential.GetTokenAsync(context);
            return tokenResponse.Token;
        }
        catch (Exception e)
        {
            Console.Out.WriteLine("{0} \n\n{1}", e.Message, e.InnerException != null ? e.InnerException.Message : "Acquire token failed");
            throw;
        }
    }
}