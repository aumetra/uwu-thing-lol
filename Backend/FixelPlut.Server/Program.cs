using FixelPlut.Server.Services;
using FixelPlut.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace FixelPlut.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Services.AddHostedService<FromFileService>();
            builder.Services.AddSingleton<FromPingPongService>();
            builder.Services.AddHostedService<FromPingPongService>(x => x.GetRequiredService<FromPingPongService>());
            builder.Services.AddSingleton<IQueueService, QueueService>();
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.Run("http://151.217.2.77:5000");
        }
    }
}
