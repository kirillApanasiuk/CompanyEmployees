using Entities.DataTransferObjects.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects.Company
{
    public class CompanyForCreationDto : CompanyForManipulatingDto
    {
        public IEnumerable<EmployeeForCreationDto> Employees { get; set; }
    }
}
