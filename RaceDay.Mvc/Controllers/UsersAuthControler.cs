using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Controllers
{
    public class UsersAuthControler :Controller
    {

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult register(RegisterView model)
        {
            if (!ModelState.IsValid)
            {

                return View("index", model);
            }
            return RedirectToAction("index", "ThankYouPage");
        }

        //[HttpGet]
        //public IActionResult Register()
        //{
        //    var model = new Register();
        //    return View(model);
        //}

        //// POST: /Auth/Register
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Register(RegisterViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // TODO: Process user registration database persistence maps here

        //    return RedirectToAction("Login");
        //}
    }
}
