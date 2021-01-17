using Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : Controller
    {
        private IRepositoryManager _repository;
        private ILoggerManager _logger;

        public CompaniesController(
            IRepositoryManager repository,
            ILoggerManager logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {
            try
            {
                var companies = _repository.Company.GetAllCompanies(false);

                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger
                    .LogError($"Something went wrong it the " +
                    $"{nameof(GetCompanies)} action {ex}");
                return StatusCode(500, "Iternal server error");
            }
        }

        
    }
}
