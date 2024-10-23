using Azure.Core;
using Azure.Identity;
using Npgsql;

namespace Ad_Poc;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //--------- Configuração do EntraId / Ad no PostgreSQL
        var connectionString = builder.Configuration["postgre:database"]!;
        var entraIdUser = builder.Configuration["postgre:entraIdUser"]!;
        var azurePostgreTokenScope = builder.Configuration["postgre:tokenScope"]!;
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Username = entraIdUser
        };

        builder.Services.AddNpgsqlDataSource(connectionStringBuilder.ToString(), dataSourceBuilder =>
        {
            dataSourceBuilder.UsePeriodicPasswordProvider(async (_, cancellationToken) =>
            {
                var azureCredential = new DefaultAzureCredential();
                var tokenScope = new TokenRequestContext([azurePostgreTokenScope]);
                var tokenResponse = await azureCredential.GetTokenAsync(tokenScope, cancellationToken);
                return tokenResponse.Token;
            }, TimeSpan.FromHours(3), TimeSpan.FromSeconds(5));
        });
        
        //--------- Fim da Configuração do PostgreSQL

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}