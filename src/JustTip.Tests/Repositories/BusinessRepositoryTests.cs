using System;
using System.Linq;
using JustTip.Core.Models;
using JustTip.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace JustTip.Tests.Repositories
{
    public class BusinessRepositoryTests : IDisposable
    {
        private readonly JustTipDbContext _context;
        private readonly BusinessRepository _repository;

        public BusinessRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JustTipDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new JustTipDbContext(options);
            _repository = new BusinessRepository(_context);
        }

        [Fact]
        public void GetById_ReturnsBusiness_WhenBusinessExists()
        {
            // Arrange
            var business = new Business 
            { 
                Id = 1, 
                Name = "Test Business"
            };

            _context.Businesses.Add(business);
            _context.SaveChanges();

            // Act
            var result = _repository.GetById(1, false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(business.Name, result.Name);
        }

        [Fact]
        public void GetById_ReturnsNull_WhenBusinessDoesNotExist()
        {
            // Act
            var result = _repository.GetById(999, false);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ReturnsAllBusinesses()
        {
            // Arrange
            var businesses = new List<Business>
            {
                new Business { Id = 1, Name = "Business 1" },
                new Business { Id = 2, Name = "Business 2" }
            };

            _context.Businesses.AddRange(businesses);
            _context.SaveChanges();

            // Act
            var result = _repository.GetAll().ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Business 1", result[0].Name);
            Assert.Equal("Business 2", result[1].Name);
        }

        [Fact]
        public async Task AddAsync_AddsBusiness_WhenValidDataProvided()
        {
            // Arrange
            var business = new Business
            {
                Name = "New Business"
            };

            // Act
            await _repository.AddAsync(business);
            await _context.SaveChangesAsync();

            // Assert
            var savedBusiness = await _context.Businesses
                .FirstOrDefaultAsync(b => b.Name == "New Business");
            Assert.NotNull(savedBusiness);
            Assert.Equal(business.Name, savedBusiness.Name);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
} 