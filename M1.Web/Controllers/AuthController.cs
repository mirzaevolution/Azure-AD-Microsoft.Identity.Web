using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

namespace M1.Web.Controllers
{
    public class AuthController : Controller
    {

        public IActionResult Login(string returnUrl="/")
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            string url = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "Home", null, Request.Scheme);
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = url
            }, OpenIdConnectDefaults.AuthenticationScheme);
        }
        [Authorize]
        public IActionResult Logout()
        {
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home", null, Request.Scheme)
            }, OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
