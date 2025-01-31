using System;
using System.Linq;
using JustTip.Core.Models;
using JustTip.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace JustTip.Tests.Repositories
{
    public class TipRepositoryTests : IDisposable
    {
        private readonly JustTipDbContext _context;
        private readonly TipRepository _repository;

        public TipRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JustTipDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new JustTipDbContext(options);
            _repository = new TipRepository(_context);
        }

        [Fact]
        public async Task AddTipAllocationAsync_AddsTipAllocation_WhenValidDataProvided()
        {
            // Arrange
            var tipAllocation = new TipAllocation
            {
                EmployeeId = 1,
                Amount = 100m,
                DistributionDate = DateTime.Today,
                PeriodStart = DateTime.Today.AddDays(-7),
                PeriodEnd = DateTime.Today
            };

            // Act
            var result = await _repository.AddTipAllocationAsync(tipAllocation);

            // Assert
            Assert.NotNull(result);
            var savedTip = await _context.TipAllocations.FirstOrDefaultAsync();
            Assert.NotNull(savedTip);
            Assert.Equal(tipAllocation.EmployeeId, savedTip.EmployeeId);
            Assert.Equal(tipAllocation.Amount, savedTip.Amount);
            Assert.Equal(tipAllocation.DistributionDate.Date, savedTip.DistributionDate.Date);
        }

        [Fact]
        public void GetTipAllocations_ReturnsTipsForEmployee_OrderedByDateDescending()
        {
            // Arrange
            var employeeId = 1;
            var tipAllocations = new List<TipAllocation>
            {
                new TipAllocation 
                { 
                    EmployeeId = employeeId,
                    Amount = 100m,
                    DistributionDate = DateTime.Today
                },
                new TipAllocation 
                { 
                    EmployeeId = employeeId,
                    Amount = 150m,
                    DistributionDate = DateTime.Today.AddDays(-1)
                },
                new TipAllocation 
                { 
                    EmployeeId = 2, // Different employee
                    Amount = 200m,
                    DistributionDate = DateTime.Today
                }
            };

            _context.TipAllocations.AddRange(tipAllocations);
            _context.SaveChanges();

            // Act
            var result = _repository.GetTipAllocations(employeeId).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(100m, result[0].Amount); // Most recent first
            Assert.Equal(150m, result[1].Amount);
            Assert.All(result, tip => Assert.Equal(employeeId, tip.EmployeeId));
        }

        [Fact]
        public void GetTipAllocations_ReturnsEmptyList_WhenNoTipsExist()
        {
            // Act
            var result = _repository.GetTipAllocations(1).ToList();

            // Assert
            Assert.Empty(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
} 