using Poll.Data.Users;
using Poll.Services.Users.ModelView;
using System.Collections.Generic;
using System.Threading.Tasks;
using Poll.Data.Model;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Poll.Services.Users
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

        public async Task RegisterAsync(RegisterViewModel model)
        {
            // On vérifie si le model n'est pas vide
            if(model.Pseudo != null && model.Email != null && model.Password != null) { 
                // On vérifie si j'ai un Email ou un Pseudo similaire en bdd
                if(_userRepo.AnyEmailOrPseudo(model.Email , model.Pseudo))
                {
                    // On rajoute dans l'entité User les informations issus du model
                    var users = new User()
                    {
                        Pseudo = model.Pseudo,
                        Email = model.Email,    
                        Password = model.Password
                    };
                    // On evoit la requête à la BDD
                    await _userRepo.AddUserAsync(users);
                }
            }
        }

        public async Task<bool> Authenticated(LoginViewModel model)
        {
            if(model.Email != null && model.Password != null)
            {
                // Vérifier si l'email est le mot de passe sont justes
                // Grace à une function dans le repo
                if (_userRepo.AuthenticatedAsync(model.Email, model.Password))
                {
                    User user = _userRepo.GetUserByEmail(model.Email);
                    // Sortir l'entité User grace à une function dans le User
                    var claims = new List<Claim>()
                    {
                        // Récupérer l'entité de l'user grace à l'email
                        new Claim("Pseudo", user.Pseudo),
                        new Claim("Email", user.Email),
                    };
                    var identity = new ClaimsIdentity(claims);
                    var pricipal = new ClaimsPrincipal(identity);
                    await _httpContext.Authentication.SignInAsync(
                        "Cookies",
                        pricipal
                        );
                    return true;
                }
            }
                return false;
            
        }
        public async Task Logout()
        {
            await _httpContext.SignOutAsync();
        }

    }
}
