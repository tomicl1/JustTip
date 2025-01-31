using AutoMapper;
using JustTip.Core.Interfaces;
using JustTip.Core.Models;
using JustTip.Web.Controllers;
using JustTip.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using JustTip.Web.Services;

namespace JustTip.Tests.Controllers
{
    public class BusinessControllerTests
    {
        private readonly Mock<IBusinessRepository> _businessRepositoryMock;
        private readonly Mock<ITipRepository> _tipRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<BusinessController>> _loggerMock;
        private readonly Mock<IBusinessService> _businessServiceMock;
        private readonly BusinessController _controller;

        public BusinessControllerTests()
        {
            _businessRepositoryMock = new Mock<IBusinessRepository>();
            _tipRepositoryMock = new Mock<ITipRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<BusinessController>>();
            _businessServiceMock = new Mock<IBusinessService>();

            _controller = new BusinessController(
                _businessRepositoryMock.Object,
                _tipRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _businessServiceMock.Object
            );
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenBusinessDoesNotExist()
        {
            // Arrange
            var businessId = 1;
            _businessServiceMock.Setup(service => service.GetBusinessDtoByIdAsync(businessId))
                .ReturnsAsync((BusinessDto?)null);

            // Act
            var result = await _controller.Details(businessId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_GET_ReturnsViewResult()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<BusinessDto>(viewResult.Model);
        }

        [Fact]
        public async Task Create_POST_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var businessDto = new BusinessDto { Name = "New Business" };
            var business = new Business();

            _mapperMock.Setup(m => m.Map<Business>(businessDto))
                .Returns(business);

            // Act
            var result = await _controller.Create(businessDto);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_POST_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var businessDto = new BusinessDto();
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Create(businessDto);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(businessDto, viewResult.Model);
        }
    }
} 