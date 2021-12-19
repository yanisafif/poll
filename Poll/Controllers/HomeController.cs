using Microsoft.AspNetCore.Mvc;
using Poll.Models;
using Poll.Services.Users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Poll.Services.Users.ModelView;

namespace Poll.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly IUsersService _usersService;

        public HomeController( IUsersService users)
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

            return Redirect("index");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await _usersService.Authenticated(model);
            if (result)
            {
                return Redirect("index");
            }

            return View();
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
