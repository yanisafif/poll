using Poll.Data.Model;
using Poll.Data.Repositories;
using Poll.Services.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Poll.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _userRepo;
        private readonly HttpContext _httpContext;

        public UsersService(IUsersRepository users, IHttpContextAccessor contextAccessor)
        {
            _userRepo = users;
            _httpContext = contextAccessor.HttpContext;
        }

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            // On vérifie si le model n'est pas vide
            if(model.Pseudo != null && model.Email != null && model.Password != null) { 
                // On vérifie si j'ai un Email ou un Pseudo similaire en bdd
                if(_userRepo.AnyEmailOrPseudo(model.Email , model.Pseudo))
                {
                    // Cryptage du mot de passe à intégrer en bdd
                    var password = model.Password;
                    var passwordSHA256 = EasyEncryption.SHA.ComputeSHA256Hash(password);
                    // On rajoute dans l'entité User les informations issus du model
                    var users = new User()
                    {
                        Pseudo = model.Pseudo,
                        Email = model.Email,    
                        Password = passwordSHA256
                    };
                    // On envoit la requête à la BDD
                    await _userRepo.AddUserAsync(users);
                    
                    var login = new LoginViewModel() { Email = model.Email, Password = model.Password };
                    await this.AuthenticatedAsync(login);

                    return true;
                }
            }
            return false;
        }

        public async Task<bool> AuthenticatedAsync(LoginViewModel model)
        {
            // On vérifie si le model n'est pas vide
            if(model.Email != null && model.Password != null)
            {
                // On récupère le repo de l'email
                User user = _userRepo.GetUserByEmail(model.Email);
                if (user != null)
                {
                    // On crypte le password pour vérifier que c'est bien celui en bdd
                    var passwordTocheck = EasyEncryption.SHA.ComputeSHA256Hash(model.Password);
                    if (passwordTocheck == user.Password)
                    {
                        var claims = new List<Claim>()
                        {
                            new Claim("Email", user.Email),
                        };
                        var identity = new ClaimsIdentity(claims, "Cookies");
                        var pricipal = new ClaimsPrincipal(identity);

                        var properties = new AuthenticationProperties()
                        {
                            IsPersistent = true
                        };

                        await _httpContext.SignInAsync(
                            "Cookies",
                            pricipal, 
                            properties
                        );
                        return true;
                    }
                }
            }
            return false;
        }

        /*
        * Cette function est utile pour récupérer l'entité de l'user quand il est connecté
        */
        public User GetUserWithClaims()
        {
            var claims = _httpContext.User.Claims.Select(c => c.Value);
            var claimsEmail = string.Join(Environment.NewLine, claims);
            User user = _userRepo.GetUserByEmail(claimsEmail);
            return user;
        }

        public bool IsUserLoggedIn()
        {
            return _httpContext.User.Identity.IsAuthenticated;
        }

        public async Task LogoutAsync()
        {
            await _httpContext.SignOutAsync();
        }

    }
}
