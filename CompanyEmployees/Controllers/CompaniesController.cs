﻿
using AutoMapper;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.ModelBinders;
using Contracts;
using Entities.DataTransferObjects;
using Entities.DataTransferObjects.Company;
using Entities.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/companies")]
    [ApiController]
    //[ResponseCache(CacheProfileName = "120SecodsDuration")]
    public class CompaniesController : ControllerBase
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

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();
        }

        [HttpGet(Name = "GetCompany")]
        [ResponseCache(Duration =60)]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await
            _repository.Company.GetAllCompaniesAsync(false);

            var companiesDto = _mapper.Map<ICollection<CompanyDto>>(companies);
            return Ok(companiesDto);
        }

        [HttpGet("{id}", Name = "CompanyById")]
        [HttpCacheExpiration(CacheLocation =CacheLocation.Public,MaxAge =60)]
        [HttpCacheValidation(MustRevalidate =false)]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await
                _repository
                .Company
                .GetCompanyAsync(id, trackChanges: false);

            if (company == null)
            {
                _logger
                .LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                var companyDto = _mapper
                    .Map<CompanyDto>(company);
                return Ok(companyDto);
            }

        }

        [HttpPost(Name ="CreateCompany")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany(
         [FromBody] CompanyForCreationDto company
        )
        {
            var companyEntity = _mapper
                .Map<Company>(company);

            _repository.Company.CreateCompany(companyEntity);
            await _repository.SaveAsync();

            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

            return CreatedAtRoute(
                "CompanyById",
                new { id = companyToReturn.Id },
                companyToReturn
            );
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection
            ([
            ModelBinder
            (BinderType =typeof(ArrayModelBinder)
            )]
            IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }

            var companyEntities = await _repository
                .Company.GetByIdsAsync(ids, false);
            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collecton");
                return NotFound();
            }

            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var company = HttpContext.Items["company"] as Company;

            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection(
            [FromBody] IEnumerable<CompanyForCreationDto> companyCollection
         )
        {
            if (companyCollection == null)
            {
                _logger.LogError("Company collection sent from client is null");
                return BadRequest("Company collection is null");
            }

            var comopanyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);

            foreach (var company in comopanyEntities)
            {
                _repository.Company.CreateCompany(company);
            }

            await _repository.SaveAsync();

            var companycollectionToReturn =
                _mapper.Map<IEnumerable<CompanyDto>>(comopanyEntities);

            var ids = string
                .Join(",", companycollectionToReturn.Select(c => c.Id));

            return CreatedAtRoute("CompanyCollection",
                new { ids },
                companycollectionToReturn);
        }


        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> UpdateCompany(
            Guid id,
            [FromBody] CompanyForUpdateDto company)
        {
            var companyEntity = HttpContext.Items["company"] as Company;
            _mapper.Map(company, companyEntity);

            await _repository.SaveAsync();

            return NoContent();
        }
    }
}

