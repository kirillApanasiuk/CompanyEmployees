using AutoMapper;
using Entities.DataTransferObjects;
using Entities.DataTransferObjects.Company;
using Entities.DataTransferObjects.Employee;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployees
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(
                dest=>dest.FullAdress,
                source=>source
                .MapFrom(source=>string
                .Join(" ",source.Country,source.Address)));

            CreateMap<Employee, EmployeeDto>().ReverseMap();

            CreateMap<EmployeeForCreationDto, Employee>();

            CreateMap<CompanyForCreationDto, Company>();

            CreateMap<EmployeeForUpdateDto, Employee>()
                .ReverseMap();
        }
    }
}
