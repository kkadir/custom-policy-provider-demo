using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomPolicyProvidersDemo.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CustomPolicyProvidersDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [Permissions(Permissions = new[] { "weather:*:*"},
                     Roles = new [] { "user", "super admin"},
                     Scopes = new[] { "App.Demo" })]
        [HttpGet("or")]
        public IEnumerable<WeatherForecast> GetWithOrPermissions()
        {
            return RandomData();
        }

        [Permissions(Permissions = new[] { "weather:forecast:modify-data" },
                     Roles = new[] { "super admin" },
                     Scopes = new[] { "App.Demo" })]
        [HttpGet("or/partial")]
        public IEnumerable<WeatherForecast> GetWithOrPermissionsPartial()
        {
            return RandomData();
        }

        [Permissions(Permissions = new[] { "weather:forecast:modify-data" },
                     Roles = new[] { "super admin" },
                     Scopes = new[] { "App.Production" })]
        [HttpGet("or/fail")]
        public IEnumerable<WeatherForecast> GetWithOrPermissionsFail()
        {
            return RandomData();
        }

        [Permissions(Permissions = new[] { "weather:*:*" })]
        [Permissions(Roles = new[] { "user", "super admin" })]
        [Permissions(Scopes = new[] { "App.Demo" })]
        [HttpGet("and")]
        public IEnumerable<WeatherForecast> GetWithAndPermissions()
        {
            return RandomData();
        }

        [Permissions(Permissions = new[] { "weather:*:*" })]
        [Permissions(Roles = new[] { "user", "super admin" })]
        [Permissions(Scopes = new[] { "App.Production" })]
        [HttpGet("and/fail")]
        public IEnumerable<WeatherForecast> GetWithAndPermissionsFail()
        {
            return RandomData();
        }

        [Permissions]
        [HttpGet("empty")]
        public IEnumerable<WeatherForecast> GetWithEmptyPermissions()
        {
            return RandomData();
        }

        private static IEnumerable<WeatherForecast> RandomData()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
