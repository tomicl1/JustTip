using AutoMapper;
using JustTip.Core.Exceptions;
using JustTip.Core.Interfaces;
using JustTip.Web.Models.DTOs;
using JustTip.Web.Models.ViewModels;
using JustTip.Web.Services;
using Moq;

namespace JustTip.Tests.Services
{
    public class BusinessServiceTests
    {
        private readonly Mock<IBusinessRepository> _businessRepositoryMock;
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BusinessService _businessService;

        public BusinessServiceTests()
        {
            _businessRepositoryMock = new Mock<IBusinessRepository>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _mapperMock = new Mock<IMapper>();
            _businessService = new BusinessService(
                _businessRepositoryMock.Object,
                _employeeRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetBusinessDtoByIdAsync_ReturnsBusinessDto_WhenBusinessExists()
        {
            // Arrange
            var businessId = 1;
            var business = new Core.Models.Business { Id = businessId, Name = "Test Business" };
            var businessDto = new BusinessDto { Id = businessId, Name = "Test Business" };

            _businessRepositoryMock.Setup(repo => repo.GetBusinessWithEmployeesAndTipsAsync(businessId))
                .ReturnsAsync(business);
            _mapperMock.Setup(mapper => mapper.Map<BusinessDto>(business))
                .Returns(businessDto);

            // Act
            var result = await _businessService.GetBusinessDtoByIdAsync(businessId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(businessId, result.Id);
            Assert.Equal("Test Business", result.Name);
        }

        [Fact]
        public async Task GetBusinessDtoByIdAsync_ThrowsBusinessNotFoundException_WhenBusinessDoesNotExist()
        {
            // Arrange
            var businessId = 1;
            _businessRepositoryMock.Setup(repo => repo.GetBusinessWithEmployeesAndTipsAsync(businessId))
                .ReturnsAsync((Core.Models.Business?)null);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessNotFoundException>(() => _businessService.GetBusinessDtoByIdAsync(businessId));
        }

        [Fact]
        public async Task CalculateTipsAsync_ReturnsCorrectTipDistribution_ForValidInput()
        {
            // Arrange
            var viewModel = new TipDistributionViewModel
            {
                BusinessId = 1,
                TotalTipAmount = 100,
                PeriodStart = new DateTime(2023, 1, 1),
                PeriodEnd = new DateTime(2023, 1, 31),
                Employees = new List<EmployeeTipViewModel>
                {
                    new EmployeeTipViewModel { EmployeeId = 1, DaysWorked = 10 },
                    new EmployeeTipViewModel { EmployeeId = 2, DaysWorked = 20 }
                }
            };

            var business = new Core.Models.Business { Id = 1, Name = "Test Business" };
            _businessRepositoryMock.Setup(repo => repo.GetByIdAsync(viewModel.BusinessId, false))
                .ReturnsAsync(business);
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeesWithShiftsAsync(viewModel.BusinessId))
                .ReturnsAsync(new List<Core.Models.Employee>
                {
                    new Core.Models.Employee { Id = 1, Shifts = new List<Core.Models.Shift>() },
                    new Core.Models.Employee { Id = 2, Shifts = new List<Core.Models.Shift>() }
                });

            // Act
            var result = await _businessService.CalculateTipsAsync(viewModel);

            // Assert
            Assert.Equal(0, result.ErrorMessage.Length);
            Assert.Equal(33.33m, result.ViewModel.Employees[0].TipAmount);
            Assert.Equal(66.67m, result.ViewModel.Employees[1].TipAmount);
        }

        [Fact]
        public async Task CalculateTipsAsync_ReturnsErrorMessage_WhenNoEmployeesHaveWorkedDays()
        {
            // Arrange
            var viewModel = new TipDistributionViewModel
            {
                BusinessId = 1,
                TotalTipAmount = 100,
                PeriodStart = new DateTime(2023, 1, 1),
                PeriodEnd = new DateTime(2023, 1, 31),
                Employees = new List<EmployeeTipViewModel>()
            };

            var business = new Core.Models.Business { Id = 1, Name = "Test Business" };
            _businessRepositoryMock.Setup(repo => repo.GetByIdAsync(viewModel.BusinessId, false))
                .ReturnsAsync(business);
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeesWithShiftsAsync(viewModel.BusinessId))
                .ReturnsAsync(new List<Core.Models.Employee>());

            // Act
            var result = await _businessService.CalculateTipsAsync(viewModel);

            // Assert
            Assert.Equal("No employees found with worked days in the selected period.", result.ErrorMessage);
        }

        [Fact]
        public async Task CalculateTipsAsync_HandlesZeroTotalTipAmount()
        {
            // Arrange
            var viewModel = new TipDistributionViewModel
            {
                BusinessId = 1,
                TotalTipAmount = 0,
                PeriodStart = new DateTime(2023, 1, 1),
                PeriodEnd = new DateTime(2023, 1, 31),
                Employees = new List<EmployeeTipViewModel>
                {
                    new EmployeeTipViewModel { EmployeeId = 1, DaysWorked = 10 },
                    new EmployeeTipViewModel { EmployeeId = 2, DaysWorked = 20 }
                }
            };

            var business = new Core.Models.Business { Id = 1, Name = "Test Business" };
            _businessRepositoryMock.Setup(repo => repo.GetByIdAsync(viewModel.BusinessId, false))
                .ReturnsAsync(business);
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeesWithShiftsAsync(viewModel.BusinessId))
                .ReturnsAsync(new List<Core.Models.Employee>
                {
                    new Core.Models.Employee { Id = 1, Shifts = new List<Core.Models.Shift>() },
                    new Core.Models.Employee { Id = 2, Shifts = new List<Core.Models.Shift>() }
                });

            // Act
            var result = await _businessService.CalculateTipsAsync(viewModel);

            // Assert
            Assert.Equal(0, result.ErrorMessage.Length);
            Assert.Equal(0, result.ViewModel.Employees[0].TipAmount);
            Assert.Equal(0, result.ViewModel.Employees[1].TipAmount);
        }

        [Fact]
        public async Task CalculateTipsAsync_ReturnsErrorMessage_WhenBusinessHasNoEmployees()
        {
            // Arrange
            var viewModel = new TipDistributionViewModel
            {
                BusinessId = 1,
                TotalTipAmount = 100,
                PeriodStart = new DateTime(2023, 1, 1),
                PeriodEnd = new DateTime(2023, 1, 31),
                Employees = new List<EmployeeTipViewModel>()
            };

            var business = new Core.Models.Business { Id = 1, Name = "Test Business", Employees = new List<Core.Models.Employee>() };
            _businessRepositoryMock.Setup(repo => repo.GetByIdAsync(viewModel.BusinessId, false))
                .ReturnsAsync(business);
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeesWithShiftsAsync(viewModel.BusinessId))
                .ReturnsAsync(new List<Core.Models.Employee>());

            // Act
            var result = await _businessService.CalculateTipsAsync(viewModel);

            // Assert
            Assert.Equal("No employees found with worked days in the selected period.", result.ErrorMessage);
        }
    }
}