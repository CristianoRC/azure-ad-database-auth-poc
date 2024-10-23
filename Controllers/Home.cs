using Azure.Core;
using Azure.Identity;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace Ad_Poc.Controllers;

[ApiController]
[Route("/")]
public class Home(IConfiguration configuration) : ControllerBase
{
    private readonly string _connectionString = configuration.GetConnectionString("database")!;
    private readonly string _pgConnectionString = configuration.GetConnectionString("pg-database");

    [HttpGet("sqlserver")]
    [AllowAnonymous]
    public async Task<IActionResult> SqlServer()
    {
        try
        {
            await using var dbConnection = new SqlConnection(_connectionString);
            const string sql =
                "SELECT TOP (10) [CustomerID], [FirstName], [LastName], [CompanyName]  FROM [SalesLT].[Customer]";
            var users = await dbConnection.QueryAsync<User>(sql);
            return Ok(users);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, StackTrack = e.StackTrace });
        }
    }

    [HttpGet("pgsql")]
    [AllowAnonymous]
    public async Task<IActionResult> PostgreSql()
    {
        try
        {
            NpgsqlConnectionStringBuilder connectionStringBuilder = new(_pgConnectionString);
            var token = await GetPgSqlToken();
            connectionStringBuilder.Username = "cristiano.cunha@akadseguros.com.br";
            connectionStringBuilder.Password = token;

            await using var conn = new NpgsqlConnection(connectionStringBuilder.ToString());
            await conn.OpenAsync();
            var response = await conn.QueryAsync<dynamic>("");
            await conn.CloseAsync();
            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, StackTrack = e.StackTrace });
        }
    }


    private static async Task<string> GetPgSqlToken()
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
            Console.Out.WriteLine("{0} \n\n{1}", e.Message,
                e.InnerException != null ? e.InnerException.Message : "Acquire token failed");
            throw;
        }
    }
}