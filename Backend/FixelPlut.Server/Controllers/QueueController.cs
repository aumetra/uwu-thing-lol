using Microsoft.AspNetCore.Mvc;

namespace FixelPlut.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly ILogger<QueueController> _logger;

        public QueueController(ILogger<QueueController> logger)
        {
            _logger = logger;
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{

        //}
    }
}
