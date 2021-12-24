using Poll.Data.Model;
using Poll.Services.Users.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poll.Services.Users
{
    public interface IUsersService
    {
        Task RegisterAsync(RegisterViewModel model);

        Task<bool> Authenticated(LoginViewModel model);
        User GetUserWithClaims();
        Task Logout();


    }
}
