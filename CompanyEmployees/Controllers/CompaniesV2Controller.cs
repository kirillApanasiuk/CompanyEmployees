using Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CompanyEmployees.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/companies")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    public class CompaniesV2Controller : Controller
    {
        private IRepositoryManager _repository;

        public CompaniesV2Controller(IRepositoryManager repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);

        }
    }
}
