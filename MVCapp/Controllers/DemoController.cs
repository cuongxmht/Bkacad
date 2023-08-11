using Microsoft.AspNetCore.Mvc;

namespace MVCapp.Controllers
{
    public class DemoController:Controller
    {
        [HttpGet]
        public IActionResult FullName()
        {
            return View();
        }
        [HttpPost]
        public IActionResult FullName(string fName)
        {
            ViewBag.Msg=$"Full name: {fName}";
            return View();
        }
    }
}