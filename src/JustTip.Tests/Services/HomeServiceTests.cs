using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JustTip.Core.Interfaces;
using JustTip.Core.Models;
using JustTip.Web.Models.DTOs;
using JustTip.Web.Services;
using Moq;
using Xunit;

namespace JustTip.Tests.Services
{
    public class HomeServiceTests
    {
        private readonly Mock<IBusinessRepository> _businessRepositoryMock;
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly HomeService _homeService;

        public HomeServiceTests()
        {
            _businessRepositoryMock = new Mock<IBusinessRepository>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _mapperMock = new Mock<IMapper>();
            _homeService = new HomeService(
                _businessRepositoryMock.Object,
                _employeeRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetDashboardDataAsync_ReturnsBusinessDtos_WhenDataExists()
        {
            // Arrange
            var businesses = new List<Business>
            {
                new Business { Id = 1, Name = "Restaurant A" },
                new Business { Id = 2, Name = "Restaurant B" }
            };

            var businessDtos = new List<BusinessDto>
            {
                new BusinessDto { Id = 1, Name = "Restaurant A" },
                new BusinessDto { Id = 2, Name = "Restaurant B" }
            };

            _businessRepositoryMock.Setup(repo =>
                repo.GetBusinessesWithEmployeesAndTipsAsync())
                .ReturnsAsync(businesses);

            _mapperMock.Setup(mapper =>
                mapper.Map<IEnumerable<BusinessDto>>(businesses))
                .Returns(businessDtos);

            // Act
            var result = await _homeService.GetDashboardDataAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Collection(result,
                dto => Assert.Equal("Restaurant A", dto.Name),
                dto => Assert.Equal("Restaurant B", dto.Name)
            );
        }

        [Fact]
        public async Task GetRecentTipAllocationsAsync_ReturnsLatestTenAllocations()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var business = new Business { Id = 1, Name = "Restaurant A" };
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                Business = business
            };

            var tipAllocations = new List<TipAllocation>
            {
                new TipAllocation
                {
                    Id = 1,
                    Employee = employee,
                    Amount = 150,
                    DistributionDate = now,
                    PeriodStart = now.AddDays(-7),
                    PeriodEnd = now
                },
                new TipAllocation
                {
                    Id = 2,
                    Employee = employee,
                    Amount = 100,
                    DistributionDate = now.AddDays(-1),
                    PeriodStart = now.AddDays(-8),
                    PeriodEnd = now.AddDays(-1)
                }
            };

            employee.TipAllocations = tipAllocations;
            business.Employees = new List<Employee> { employee };

            var businesses = new List<Business> { business };

            _businessRepositoryMock.Setup(repo =>
                repo.GetBusinessesWithEmployeesAndTipsAsync())
                .ReturnsAsync(businesses);

            // Act
            var result = await _homeService.GetRecentTipAllocationsAsync();

            // Assert
            var allocations = result.ToList();
            Assert.Equal(2, allocations.Count);

            // Check first (most recent) allocation
            Assert.Equal(150, allocations[0].Amount);
            Assert.Equal(now, allocations[0].DistributionDate);

            // Check second allocation
            Assert.Equal(100, allocations[1].Amount);
            Assert.Equal(now.AddDays(-1), allocations[1].DistributionDate);

            // Check common properties for all allocations
            Assert.All(allocations, a =>
            {
                Assert.Equal("Restaurant A", a.BusinessName);
                Assert.Equal("John Doe", a.EmployeeName);
            });
        }

        [Fact]
        public async Task GetDashboardDataAsync_ReturnsEmptyList_WhenNoBusinessesExist()
        {
            // Arrange
            var emptyList = new List<Business>();
            var emptyDtos = new List<BusinessDto>();

            _businessRepositoryMock.Setup(repo =>
                repo.GetBusinessesWithEmployeesAndTipsAsync())
                .ReturnsAsync(emptyList);

            _mapperMock.Setup(mapper =>
                mapper.Map<IEnumerable<BusinessDto>>(emptyList))
                .Returns(emptyDtos);

            // Act
            var result = await _homeService.GetDashboardDataAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetRecentTipAllocationsAsync_ReturnsEmptyList_WhenNoTipAllocationsExist()
        {
            // Arrange
            var business = new Business { Id = 1, Name = "Restaurant A" };
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                Business = business,
                TipAllocations = new List<TipAllocation>() // Empty tip allocations
            };

            business.Employees = new List<Employee> { employee };
            var businesses = new List<Business> { business };

            _businessRepositoryMock.Setup(repo =>
                repo.GetBusinessesWithEmployeesAndTipsAsync())
                .ReturnsAsync(businesses);

            // Act
            var result = await _homeService.GetRecentTipAllocationsAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}