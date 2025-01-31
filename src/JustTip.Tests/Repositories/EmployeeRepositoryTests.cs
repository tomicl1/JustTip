using JustTip.Core.Models;
using JustTip.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JustTip.Tests.Repositories
{
    public class EmployeeRepositoryTests : IDisposable
    {
        private readonly JustTipDbContext _context;
        private readonly EmployeeRepository _repository;

        public EmployeeRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JustTipDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new JustTipDbContext(options);
            _repository = new EmployeeRepository(_context);

            // Seed the database with a business
            var business = new Business { Id = 1, Name = "Test Business" };
            _context.Businesses.Add(business);
            _context.SaveChanges();
        }

        [Fact]
        public void GetEmployee_ReturnsEmployee_WhenEmployeeExists()
        {
            // Arrange
            var employee = new Employee
            {
                BusinessId = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Position = "Developer"
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();

            // Act
            var result = _repository.GetEmployee(1, employee.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(employee.Name, result.Name);
            Assert.Equal(employee.Email, result.Email);
        }

        [Fact]
        public void GetEmployee_ReturnsNull_WhenEmployeeDoesNotExist()
        {
            // Act
            var result = _repository.GetEmployee(1, 999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddEmployeeAsync_AddsNewEmployee_WhenValidDataProvided()
        {
            // Arrange
            var employee = new Employee
            {
                Name = "Jane Doe",
                Email = "jane@example.com",
                Position = "Manager"
            };

            // Act
            await _repository.AddEmployeeAsync(1, employee);
            await _context.SaveChangesAsync();

            // Assert
            var savedEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Name == "Jane Doe");
            
            Assert.NotNull(savedEmployee);
            Assert.Equal(employee.Email, savedEmployee.Email);
            Assert.Equal(1, savedEmployee.BusinessId);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}