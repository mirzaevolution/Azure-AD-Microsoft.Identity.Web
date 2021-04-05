using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAuth.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{errorCode}")]
        public IActionResult Index(string errorCode="404")
        {
            string errorMessage = "An error occured!";
            switch (errorCode)
            {
                case "401":
                    {
                        errorMessage = "Access denied 401";
                        break;
                    }
                case "404":
                    {
                        errorMessage = "Page you are looking for is not found";
                        break;
                    }
                default:
                    errorMessage = "Internal server error";
                    break;
            }
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }
    }
}
