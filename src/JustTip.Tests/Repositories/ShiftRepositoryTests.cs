using System;
using System.Linq;
using JustTip.Core.Models;
using JustTip.Core.Exceptions;
using JustTip.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace JustTip.Tests.Repositories
{
    public class ShiftRepositoryTests : IDisposable
    {
        private readonly JustTipDbContext _context;
        private readonly ShiftRepository _repository;

        public ShiftRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<JustTipDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new JustTipDbContext(options);
            _repository = new ShiftRepository(_context);
        }

        [Fact]
        public void GetShifts_ReturnsAllShifts()
        {
            // Arrange
            var shifts = new List<Shift>
            {
                new Shift 
                { 
                    Id = 1, 
                    EmployeeId = 1, 
                    Year = 2024,
                    Month = 3,
                    WorkedDays = "1,2,3"
                },
                new Shift 
                { 
                    Id = 2, 
                    EmployeeId = 1,
                    Year = 2024,
                    Month = 3,
                    WorkedDays = "15,16,17"
                }
            };

            _context.Shifts.AddRange(shifts);
            _context.SaveChanges();

            // Act
            var result = _repository.GetShifts().ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("1,2,3", result[0].WorkedDays);
            Assert.Equal("15,16,17", result[1].WorkedDays);
        }

        [Fact]
        public void AddShift_AddsNewShift_WhenValidDataProvided()
        {
            // Arrange
            var shift = new Shift
            {
                EmployeeId = 1,
                Year = 2024,
                Month = 3,
                WorkedDays = "1,2,3,4,5"
            };

            // Act
            _repository.AddShift(shift);

            // Assert
            var savedShift = _context.Shifts.FirstOrDefault();
            Assert.NotNull(savedShift);
            Assert.Equal(shift.EmployeeId, savedShift.EmployeeId);
            Assert.Equal(shift.Year, savedShift.Year);
            Assert.Equal(shift.Month, savedShift.Month);
            Assert.Equal(shift.WorkedDays, savedShift.WorkedDays);
        }

        [Fact]
        public void RemoveShift_RemovesExistingShift()
        {
            // Arrange
            var shift = new Shift
            {
                EmployeeId = 1,
                Year = 2024,
                Month = 3,
                WorkedDays = "1,2,3"
            };
            _context.Shifts.Add(shift);
            _context.SaveChanges();

            // Act
            _repository.RemoveShift(shift);

            // Assert
            Assert.Empty(_context.Shifts);
        }

        [Fact]
        public void GetShifts_ReturnsWorkedDaysAsList()
        {
            // Arrange
            var shift = new Shift
            {
                EmployeeId = 1,
                Year = 2024,
                Month = 3,
                WorkedDays = "1,2,3,15,22"
            };
            _context.Shifts.Add(shift);
            _context.SaveChanges();

            // Act
            var result = _repository.GetShifts().First();
            var workedDays = result.GetWorkedDays();

            // Assert
            Assert.Equal(5, workedDays.Count);
            Assert.Contains(1, workedDays);
            Assert.Contains(22, workedDays);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
} 