using Entities.DataTransferObjects.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DataTransferObjects.Company
{
    public  abstract  class CompanyForManipulatingDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }


    }
}
