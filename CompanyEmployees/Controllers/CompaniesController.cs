using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
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
        private IMapper _mapper;

        public CompaniesController(
            IRepositoryManager repository,
            ILoggerManager logger,
            IMapper mapper
        )
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {
            try
            {
                var companies = _repository.Company.GetAllCompanies(false);

                /*  var companiesDto = companies.Select(c => new CompanyDto
                  {
                      Id = c.Id,
                      Name = c.Name,
                      FullAdress = string.Join("", c.Address, c.Country)
                  }); ;*/
                var companisDto = _mapper.Map<List<CompanyDto>>(companies);
                return Ok(companisDto);
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
