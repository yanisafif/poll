using Microsoft.AspNetCore.Mvc;
using Poll.Models;
using Poll.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Poll.Services.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace Poll.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly IUsersService _usersService;

        public HomeController(IUsersService users)
        {
            _usersService = users;
        }

        public IActionResult Index()
        {
            
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            await _usersService.RegisterAsync(model);

            return Redirect("Login");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            
            if (await _usersService.Authenticated(model))
            {
                return Redirect("index");
            }else
            {
                ModelState.AddModelError("model", "L'email ou le mot de passe ne correspondent pas");
                return View();
            }

        }

        [HttpGet]
        [Authorize]
        public IActionResult Privacy()
        {
            var userConnected = _usersService.GetUserWithClaims();
            ViewData["Claims"] = userConnected.Email;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _usersService.Logout();
            return LocalRedirect("/home/index");
        }
    }
}
