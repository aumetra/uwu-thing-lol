using FixelPlut.Server.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FixelPlut.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHostedService<FromFileService>();
            builder.Services.AddSingleton<IQueueService, QueueService>();
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
