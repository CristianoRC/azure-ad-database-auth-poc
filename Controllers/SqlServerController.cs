using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Ad_Poc.Controllers;

[ApiController]
[Route("/sqlserver")]
public class SqlServerController(IConfiguration configuration) : ControllerBase
{
    private readonly string _connectionString = configuration.GetConnectionString("database")!;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> SqlServer()
    {
        try
        {
            await using var dbConnection = new SqlConnection(_connectionString);
            const string sql = "SELECT TOP (10) [CustomerID], [FirstName], [LastName], [CompanyName]  FROM [SalesLT].[Customer]";
            var users = await dbConnection.QueryAsync<User>(sql);
            return Ok(users);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, StackTrack = e.StackTrace });
        }
    }
}