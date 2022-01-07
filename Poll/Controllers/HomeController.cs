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

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userConnected = _usersService.GetUserWithClaims();
                ViewData["Claims"] = userConnected.Pseudo;
            }

            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }else{
                return Redirect("index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var resultRegistration = await _usersService.RegisterAsync(model);
            if(resultRegistration)
            {
                return Redirect("Login");
            }else{
                ModelState.AddModelError("model", "Le pseudo ou l'email est déjà utilisé");
                return View();
            }
            
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }else{
                return Redirect("index");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            
            if (await _usersService.AuthenticatedAsync(model))
            {
                return Redirect("index");
            }else
            {
                ModelState.AddModelError("model", "L'email ou le mot de passe ne correspondent pas");
                return View();
            }

        }

        [HttpGet]
        public IActionResult Privacy()
        {
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
            await _usersService.LogoutAsync();
            return LocalRedirect("/home/index");
        }
    }
}
