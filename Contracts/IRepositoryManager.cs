using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    interface IRepositoryManager
    {
        ICompanyRepository CompanyRepo { get; }
        IEmployeeRepository EmployeeRepo { get}
    } 
}
