using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Ad_Poc.Controllers;

[ApiController]
[Route("/postgresql-injection")]
public class PostgreInjectionController(NpgsqlConnection  dbConnection) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> PostgreSql()
    {
        try
        {
            const string query = "SELECT schemaname, tablename FROM pg_catalog.pg_tables LIMIT 5;";
            var response = await dbConnection.QueryAsync<dynamic>(query);
            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message, StackTrack = e.StackTrace });
        }
    }
}