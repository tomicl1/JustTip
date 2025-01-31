using JustTip.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace JustTip.Web.Controllers
{
    public interface IEmployeeController
    {
        IActionResult Index(int businessId);
        IActionResult Details(int businessId, int employeeId);
        IActionResult Create(int businessId);
        Task<IActionResult> Create(EmployeeDto employeeDto);
        IActionResult Edit(int businessId, int employeeId);
        Task<IActionResult> Edit(int businessId, EmployeeDto employeeDto);
        IActionResult Delete(int businessId, int employeeId);
        Task<IActionResult> DeleteConfirmed(int businessId, int employeeId);
    }
} 