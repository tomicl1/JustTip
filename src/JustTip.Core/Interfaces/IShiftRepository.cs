using JustTip.Core.Models;

namespace JustTip.Core.Interfaces
{
    public interface IShiftRepository
    {
        IQueryable<Shift> GetShifts();
        void AddShift(Shift shift);
        void RemoveShift(Shift shift);
    }
} 