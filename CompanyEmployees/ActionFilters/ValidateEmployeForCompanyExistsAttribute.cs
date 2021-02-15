using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees.ActionFilters
{
    public class ValidateEmployeForCompanyExistsAttribute : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _logger;

        public ValidateEmployeForCompanyExistsAttribute(
            IRepositoryManager repositoryManager,
            ILoggerManager logger
        )
        {
            _repositoryManager = repositoryManager;
            _logger = logger;

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var trackchanges = (method.Equals("PUT") || method.Equals("PATCH"))
                ? true : false;

            var companyId = (Guid)context.ActionArguments["companyId"];

            var company = await _repositoryManager.Company.GetCompanyAsync(companyId, false);


            if (company == null)
            {

                _logger.LogInfo($"Company with id: " +
                    $"{companyId} doesn't exist in the datab");

                context.Result = new NotFoundResult();

                return;
            }

            var id = (Guid)context.ActionArguments["id"];
            var employee = await _repositoryManager.Employee.GetEmployeeAsync(companyId, id, trackchanges);
            if (employee == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("employee", employee);
                await next();
            }


        }
    }
}
