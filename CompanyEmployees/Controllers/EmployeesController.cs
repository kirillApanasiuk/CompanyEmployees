
using AutoMapper;
using CompanyEmployees.ActionFilters;
using Contracts;
using Entities.DataTransferObjects;
using Entities.DataTransferObjects.Employee;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompanyEmployees.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        public readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IDataShaper<EmployeeDto> _dataShaper;

        public EmployeesController(
            IRepositoryManager repository,
            ILoggerManager logger,
            IMapper mapper,
            IDataShaper<EmployeeDto> dataShaper
            )
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _dataShaper = dataShaper;

        }

        [HttpGet("{id}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company =  await _repository.Company.GetCompanyAsync(companyId, false);

            if (company == null)
            {
                _logger.LogInfo($"Company with id:" +
                    $" {companyId} doesn't exist it the database.");
                return NotFound();
            }

            var employeeDb =
               await  _repository.Employee.GetEmployeeAsync(companyId, id, false);

            if (employeeDb == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database");
                return NotFound();
            }

            var employee = _mapper.Map<EmployeeDto>(employeeDb);
            return Ok(employee);
        }


        [HttpPost]
        public async  Task<IActionResult> CreateEmployee(Guid companyid, EmployeeForCreationDto employeeDto)
        {

            if (employeeDto == null)
            {
                _logger
                    .LogError("EmpoyeeForCreationDto object sent from client is null.");
                return BadRequest("EmployeeForCreation object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger
                    .LogError("Invalid model state for the " +
                    "EmployeeForCreationDto object");
                return UnprocessableEntity(ModelState);

            }

            var company =await  _repository.Company.GetCompanyAsync(companyid, false);

            if (company == null) {
                _logger
                    .LogInfo($"Company with id: {companyid} doesn't exist it the database.");
                return NotFound();
            }

            var employee = _mapper.Map<EmployeeForCreationDto, Employee>(employeeDto);

            _repository.Employee.CreateEmployee(companyid, employee);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employee);
            return CreatedAtRoute("GetEmployeeForCompany",
                new { companyid, id = employeeToReturn.Id },
                employeeToReturn);
        }

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetEmployeesForCompany(
          Guid companyId,
          [FromQuery] EmployeeParameters employeeParameters
        )
        {

            if (!employeeParameters.ValidAgeRange)
            {
                return BadRequest("Max age can't be less than min age");
            }
            var company = await _repository
                .Company
                .GetCompanyAsync(companyId, trackChanges: false);

            if (company == null)
            {

                _logger
                    .LogInfo($"Company with id: {companyId} " +
                    $"doesn't exist in the databese");
                return NotFound();
            }

            var employeesFromDB = await _repository
                .Employee
                .GetEmployeesAsync(companyId,employeeParameters,trackChanges: false);

            Response.Headers.Add("X-Pagination", 
                JsonConvert.SerializeObject(employeesFromDB.MetaData));
            var employeesDto = _mapper
                .Map<IEnumerable<EmployeeDto>>
                (employeesFromDB);

            return Ok(_dataShaper.ShapeData(employeesDto,employeeParameters.Fields));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId,Guid id)
        {
            var employeeForCompany = HttpContext.Items["employee"] as Employee;

            _repository.Employee.DeleteEmployee(employeeForCompany);

            await _repository.SaveAsync();


            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeForCompany(
            Guid companyId,
            Guid id,
            [FromBody] EmployeeForUpdateDto employee)
        {
            if (employee == null)
            {
                _logger
                    .LogError("EmployeeForUpdateDto object send from client is null.");
                return BadRequest("EmployeeForUpdateDto object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the EmployeeForUpdateDto object");
                return UnprocessableEntity(ModelState);
            }
            var company = await _repository.Company.GetCompanyAsync(companyId, false);
            if(company == null)
            {
                _logger
                    .LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var employeeEntity = _repository.Employee.GetEmployeeAsync(companyId, id, true);

            if(employeeEntity == null)
            {
                _logger.LogInfo($"Employee with id: {id} doen't exist in the database.");
                return NotFound();
            }

            await _mapper.Map(employee, employeeEntity);
            await _repository.SaveAsync();

            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdateEmployeeForCompany(Guid companyId, Guid id,
            [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        { 
            if(patchDoc == null)
            {
                _logger.LogError("patchDoc object send from client is null.");
                return BadRequest("patchDoc object is null");
            }

            var company = await _repository.Company.GetCompanyAsync(companyId, false);

            if(company == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeEntity = await _repository.Employee
                .GetEmployeeAsync(companyId, id, true);
            if(employeeEntity == null)
            {
                _logger.LogInfo($"Employee with id {id} doesn't exist in the database");
                return NotFound();
            }

            var employeeToPatch = _mapper
                .Map<EmployeeForUpdateDto>(employeeEntity);


            patchDoc.ApplyTo(employeeToPatch, ModelState);
            TryValidateModel(employeeToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(employeeToPatch, employeeEntity);
           await  _repository.SaveAsync();
            return NoContent();
        }
        
        [HttpPost("all")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployees(Guid companyId,[FromBody] IEnumerable<EmployeeForCreationDto> employeesForCreate)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, false);

            if (company == null)
            {
                _logger
                    .LogInfo($"Company with id: {companyId} doesn't exist it the database.");
                return NotFound();
            }
            var employees = _mapper.Map<IEnumerable<Employee>>(employeesForCreate);

            _repository.Employee.CreateEmployees(companyId,employees);

           await _repository.SaveAsync();

            var employeeForReturn = _mapper.Map<IEnumerable<EmployeeDto>>(employees);

            return Ok(employeeForReturn);
        }
    }

}
