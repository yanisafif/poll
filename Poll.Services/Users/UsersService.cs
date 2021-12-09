using Poll.Data;
using Poll.Data.Users;
using Poll.Services.Users.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poll.Data.Model;

namespace Poll.Services.Users
{
    public class UsersService : IUsersService
    {
        private readonly AppDbContext _context;
        private readonly IUsersRepository _userRepo;
        public UsersService(AppDbContext db, IUsersRepository users)
        {
            _context = db;
            _userRepo = users;
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

    }
}
