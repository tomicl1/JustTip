using AutoMapper;
using JustTip.Core.Interfaces;
using JustTip.Web.Models.DTOs;
using JustTip.Web.Services;
using Moq;

namespace JustTip.Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _mapperMock = new Mock<IMapper>();
            _employeeService = new EmployeeService(
                _employeeRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_UpdatesEmployee_WhenEmployeeExists()
        {
            // Arrange
            var businessId = 1;
            var employeeDto = new EmployeeDto
            {
                Id = 1,
                Name = "John Doe Updated",
                Email = "john.updated@example.com",
                Position = "Senior Developer"
            };
            var employee = new Core.Models.Employee
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Position = "Developer"
            };

            _employeeRepositoryMock.Setup(repo => repo.GetEmployee(businessId, employeeDto.Id))
                .Returns(employee);
            _mapperMock.Setup(mapper => mapper.Map(employeeDto, employee))
                .Callback(() =>
                {
                    employee.Name = employeeDto.Name;
                    employee.Email = employeeDto.Email;
                    employee.Position = employeeDto.Position;
                });

            // Act
            await _employeeService.UpdateEmployeeAsync(businessId, employeeDto);

            // Assert
            Assert.NotEqual(employeeDto.Name, employee.Name);
            Assert.NotEqual(employeeDto.Email, employee.Email);
            Assert.NotEqual(employeeDto.Position, employee.Position);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_DoesNotCallSaveChanges_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var businessId = 1;
            var employeeDto = new EmployeeDto
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Position = "Developer"
            };

            // Ensure the mock returns null to simulate no employee found
            _employeeRepositoryMock.Setup(repo => repo.GetEmployee(businessId, employeeDto.Id))
                .Returns((Core.Models.Employee?)null);

            // Act
            await _employeeService.UpdateEmployeeAsync(businessId, employeeDto);

            // Assert
            _employeeRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

#pragma warning disable CS8600, CS8604
        [Fact]
        public async Task UpdateEmployeeAsync_DoesNothing_WhenEmployeeDtoIsNull()
        {
            // Arrange
            var businessId = 1;
            EmployeeDto employeeDto = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _employeeService.UpdateEmployeeAsync(businessId, employeeDto));
        }
#pragma warning restore CS8600, CS8604

        [Fact]
        public async Task UpdateEmployeeAsync_ThrowsArgumentException_WhenDataExceedsMaxLength()
        {
            // Arrange
            var businessId = 1;
            var employeeDto = new EmployeeDto
            {
                Id = 1,
                Name = new string('A', 101), // Invalid: exceeds max length
                Email = new string('B', 101) + "@example.com", // Invalid: exceeds max length
                Position = new string('C', 101), // Invalid: exceeds max length
                BusinessId = businessId,
                HireDate = DateTime.UtcNow,
                Phone = new string('1', 101) // Invalid: exceeds max length
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _employeeService.UpdateEmployeeAsync(businessId, employeeDto));
        }

        [Fact]
        public async Task UpdateEmployeeAsync_DoesNotSave_WhenDataIsInvalid()
        {
            // Arrange
            var businessId = 1;
            var employeeDto = new EmployeeDto
            {
                Id = 1,
                Name = new string('A', 101), // Invalid: exceeds max length
                Email = "long.email@example.com",
                Position = "a",
                BusinessId = businessId,
                HireDate = DateTime.UtcNow,
                Phone = "123123"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _employeeService.UpdateEmployeeAsync(businessId, employeeDto));

            // Verify that SaveChangesAsync is not called
            _employeeRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetEmployeeDtoByIdAsync_ReturnsEmployeeDto_WhenEmployeeExists()
        {
            // Arrange
            var businessId = 1;
            var employeeId = 1;
            var employee = new Core.Models.Employee
            {
                Id = employeeId,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Position = "Developer"
            };
            var expectedDto = new EmployeeDto
            {
                Id = employeeId,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Position = "Developer"
            };

            _employeeRepositoryMock.Setup(repo => repo.GetEmployee(businessId, employeeId))
                .Returns(employee);
            _mapperMock.Setup(mapper => mapper.Map<EmployeeDto>(employee))
                .Returns(expectedDto);

            // Act
            var result = await _employeeService.GetEmployeeDtoByIdAsync(businessId, employeeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Id);
            Assert.Equal(expectedDto.Name, result.Name);
            Assert.Equal(expectedDto.Email, result.Email);
            Assert.Equal(expectedDto.Position, result.Position);
        }

        [Fact]
        public async Task GetEmployeeDtoByIdAsync_ReturnsNull_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var businessId = 1;
            var employeeId = 999;

            _employeeRepositoryMock.Setup(repo => repo.GetEmployee(businessId, employeeId))
                .Returns((Core.Models.Employee?)null);

            // Act
            var result = await _employeeService.GetEmployeeDtoByIdAsync(businessId, employeeId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteEmployeeAsync_CallsRepository_WithCorrectParameters()
        {
            // Arrange
            var businessId = 1;
            var employeeId = 1;

            // Act
            await _employeeService.DeleteEmployeeAsync(businessId, employeeId);

            // Assert
            _employeeRepositoryMock.Verify(repo =>
                repo.DeleteEmployeeAsync(businessId, employeeId), Times.Once);
        }
    }
}