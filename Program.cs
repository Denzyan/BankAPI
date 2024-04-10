using BankApi.Context;
using BankApi.Models;
using BankApi.Services;

namespace BankApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var env = builder.Environment.EnvironmentName;

            builder.Configuration
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Add services to the container.

            var configuration = builder.Configuration;

            builder.Services.AddControllers();

            builder.Services.AddEntityFrameworkSqlite()
                .AddDbContext<BankContext>();

            var serviceProvide = builder.Services.BuildServiceProvider();
            var context = serviceProvide.GetRequiredService<BankContext>();
            context.Database.EnsureCreated();

            // DI
            builder.Services.AddSingleton<IAccountsService, AccountsService>();
            builder.Services.AddSingleton<ITransactionService, TransactionService>();

            builder.Services.AddEndpointsApiExplorer();

            // Add Cors
            builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.MapControllers();

            app.Run();
        }
    }
}
