using AutoMapper;
using JustTip.Core.Interfaces;
using JustTip.Core.Models;
using JustTip.Web.Controllers;
using JustTip.Web.Models.DTOs;
using JustTip.Web.Models.ViewModels;
using JustTip.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace JustTip.Tests.Controllers
{
    public class EmployeeControllerTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IBusinessRepository> _businessRepositoryMock;
        private readonly Mock<IShiftRepository> _shiftRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<EmployeeController>> _loggerMock;
        private readonly Mock<IEmployeeService> _employeeServiceMock;
        private readonly EmployeeController _controller;

        public EmployeeControllerTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _businessRepositoryMock = new Mock<IBusinessRepository>();
            _shiftRepositoryMock = new Mock<IShiftRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<EmployeeController>>();
            _employeeServiceMock = new Mock<IEmployeeService>();

            _controller = new EmployeeController(
                _employeeRepositoryMock.Object,
                _businessRepositoryMock.Object,
                _shiftRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _employeeServiceMock.Object
            );
        }

        [Fact]
        public void Index_ReturnsViewResult_WhenBusinessExists()
        {
            // Arrange
            var businessId = 1;
            var business = new Business { Id = businessId, Name = "Test Business" };
            var employees = new List<Employee>();

            _businessRepositoryMock.Setup(repo => repo.GetById(businessId, false))
                .Returns(business);
            _employeeRepositoryMock.Setup(repo => repo.GetEmployees(businessId))
                .Returns(employees.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<EmployeeDto>>(employees))
                .Returns(new List<EmployeeDto>());

            // Act
            var result = _controller.Index(businessId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EmployeeListViewModel>(viewResult.Model);
            Assert.Equal(businessId, model.BusinessId);
            Assert.Equal(business.Name, model.BusinessName);
        }

        [Fact]
        public void Details_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var businessId = 1;
            var employeeId = 1;

            _employeeRepositoryMock.Setup(repo => repo.GetEmployee(businessId, employeeId))
                .Returns((Employee?)null);

            // Act
            var result = _controller.Details(businessId, employeeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var businessId = 1;
            var employeeDto = new EmployeeDto
            {
                BusinessId = businessId,
                Name = "John Doe",
                Email = "john@example.com",
                Position = "Developer"
            };

            var employee = new Employee();
            _mapperMock.Setup(m => m.Map<Employee>(employeeDto))
                .Returns(employee);

            // Act
            var result = await _controller.Create(employeeDto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(businessId, redirectResult?.RouteValues?["businessId"]);
        }

        [Fact]
        public void Edit_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var businessId = 1;
            var employeeId = 1;
            _employeeRepositoryMock.Setup(repo => repo.GetEmployee(businessId, employeeId))
                .Returns((Employee?)null);

            // Act
            var result = _controller.Edit(businessId, employeeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}