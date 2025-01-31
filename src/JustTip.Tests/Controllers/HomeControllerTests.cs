using AutoMapper;
using JustTip.Core.Interfaces;
using JustTip.Web.Controllers;
using JustTip.Web.Models;
using JustTip.Web.Models.DTOs;
using JustTip.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace JustTip.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IBusinessRepository> _businessRepositoryMock;
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly Mock<IHomeService> _homeServiceMock;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _businessRepositoryMock = new Mock<IBusinessRepository>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<HomeController>>();
            _homeServiceMock = new Mock<IHomeService>();

            _controller = new HomeController(
                _businessRepositoryMock.Object,
                _employeeRepositoryMock.Object,
                _loggerMock.Object,
                _homeServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public void Index_ReturnsViewResult_WithBusinessList()
        {
            // Arrange
            var businessDtos = new List<BusinessDto>
            {
                new BusinessDto { Id = 1, Name = "Business 1", EmployeeCount = 5 },
                new BusinessDto { Id = 2, Name = "Business 2", EmployeeCount = 3 }
            };

            _homeServiceMock.Setup(service => service.GetDashboardDataAsync())
                .ReturnsAsync(businessDtos);

            _homeServiceMock.Setup(service => service.GetRecentTipAllocationsAsync())
                .ReturnsAsync(new List<TipAllocationSummaryDto>());

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BusinessDto>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Index_HandlesException_ReturnsViewWithEmptyList()
        {
            // Arrange
            _homeServiceMock.Setup(service => service.GetDashboardDataAsync())
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BusinessDto>>(viewResult.Model);
            Assert.Empty(model);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsViewWithCorrectName()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }
    }
} 