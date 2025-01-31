using JustTip.Core.Interfaces;
using JustTip.Core.Models;
using JustTip.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace JustTip.Infrastructure.Data
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly JustTipDbContext _context;

        public ShiftRepository(JustTipDbContext context)
        {
            _context = context;
        }

        public IQueryable<Shift> GetShifts()
        {
            return _context.Shifts;
        }

        public void AddShift(Shift shift)
        {
            try
            {
                _context.Shifts.Add(shift);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new ShiftOperationException("An error occurred while adding the shift.", ex);
            }
        }

        public void RemoveShift(Shift shift)
        {
            try
            {
                _context.Shifts.Remove(shift);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new ShiftOperationException("An error occurred while removing the shift.", ex);
            }
        }
    }
} 