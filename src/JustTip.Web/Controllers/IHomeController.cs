using Microsoft.AspNetCore.Mvc;

namespace JustTip.Web.Controllers
{
    public interface IHomeController
    {
        IActionResult Index();
        IActionResult Privacy();
        IActionResult Error();
    }
} 