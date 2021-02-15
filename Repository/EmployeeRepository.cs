using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
   public class EmployeeRepository:RepositoryBase<Employee>,IEmployeeRepository
    {
        private RepositoryContext _repository;

        public EmployeeRepository(RepositoryContext repository):base(repository)
        {
            _repository = repository;

        }

        public void CreateEmployee(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void CreateEmployees(Guid companyId, IEnumerable<Employee> employees)
        {
            foreach (var employee in employees)
            {
                employee.CompanyId = companyId;
            }

            _repository.Set<Employee>().AddRange(employees);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        => await FindByCondition(e => 
        e.CompanyId.Equals(companyId) 
        &&
        e.Id.Equals(id),trackChanges)
        .SingleOrDefaultAsync();

        public async Task<PagedList<Employee>> GetEmployeesAsync(
            Guid companyId, 
            EmployeeParameters employeeParameters, 
            bool trackChanges)
        {
            var employees = await
                FindByCondition(e =>
                    e.CompanyId.Equals(companyId), trackChanges)
                .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
                .Search(employeeParameters.SearchTerm)
                .Sort(employeeParameters.OrderBy)
                .ToListAsync();
               

            return PagedList<Employee>
                .ToPagedList(employees, 
                employeeParameters.PageNumber,
                employeeParameters.PageSize);

        }
           
    }
    
}
