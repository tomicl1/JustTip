using AutoMapper;
using JustTip.Core.Models;
using JustTip.Web.Models.DTOs;

namespace JustTip.Web.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Business, BusinessDto>()
                .ForMember(dest => dest.EmployeeCount, 
                    opt => opt.MapFrom(src => src.GetEmployeeCount()));

            CreateMap<BusinessDto, Business>();
            CreateMap<Employee, EmployeeDto>();
            CreateMap<EmployeeDto, Employee>();
            CreateMap<TipAllocation, TipAllocationDto>();
        }
    }
} 