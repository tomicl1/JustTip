using AutoMapper;
using JustTip.Core.Interfaces;
using JustTip.Core.Models;
using JustTip.Web.Models.DTOs;

namespace JustTip.Web.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository repository, IMapper mapper)
        {
            _employeeRepository = repository;
            _mapper = mapper;
        }

        public async Task<EmployeeDto?> GetEmployeeDtoByIdAsync(int businessId, int employeeId)
        {
            var employee = await Task.Run(() => _employeeRepository.GetEmployee(businessId, employeeId));
            return employee == null ? null : _mapper.Map<EmployeeDto>(employee);
        }

        public async Task AddEmployeeAsync(int businessId, EmployeeDto employeeDto)
        {
            // Validate input data
            if (string.IsNullOrWhiteSpace(employeeDto.Name) || employeeDto.Name.Length > 100)
                throw new ArgumentException("Name is required and must be 100 characters or less.");
            if (!string.IsNullOrWhiteSpace(employeeDto.Email) && employeeDto.Email.Length > 100)
                throw new ArgumentException("Email must be 100 characters or less.");
            if (string.IsNullOrWhiteSpace(employeeDto.Position) || employeeDto.Position.Length > 100)
                throw new ArgumentException("Position is required and must be 100 characters or less.");

            var employee = _mapper.Map<Employee>(employeeDto);
            await _employeeRepository.AddEmployeeAsync(businessId, employee);
        }

        public async Task UpdateEmployeeAsync(int businessId, EmployeeDto employeeDto)
        {
            // Validate input data
            if (string.IsNullOrWhiteSpace(employeeDto.Name) || employeeDto.Name.Length > 100)
                throw new ArgumentException("Name is required and must be 100 characters or less.");
            if (!string.IsNullOrWhiteSpace(employeeDto.Email) && employeeDto.Email.Length > 100)
                throw new ArgumentException("Email must be 100 characters or less.");
            if (string.IsNullOrWhiteSpace(employeeDto.Position) || employeeDto.Position.Length > 100)
                throw new ArgumentException("Position is required and must be 100 characters or less.");

            var employee = _mapper.Map<Employee>(employeeDto);
            await _employeeRepository.UpdateEmployeeAsync(employee);
        }

        public async Task DeleteEmployeeAsync(int businessId, int employeeId)
        {
            await _employeeRepository.DeleteEmployeeAsync(businessId, employeeId);
        }
    }
} 