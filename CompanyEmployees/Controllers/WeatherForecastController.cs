﻿using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Controllers
{
    [Route("[controller]")] 
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        private ILoggerManager _logger;
        private IRepositoryManager _repository;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly",
            "Cool", "Mild", "Warm", "Balmy", 
            "Hot", "Sweltering", "Scorching"
        };


        public WeatherForecastController(
            ILoggerManager logger,
            IRepositoryManager repository
            )
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInfo("Here is info message from our values controller.");

            _logger.LogDebug("Here is debuf message from our valurs contorller");

            _logger.LogWarn("Here is warn message from our values controller.");

            _logger.LogError("Here is an error message from our values controller");

            return new string[] { "value1", "value2" };
        }
    }
}
