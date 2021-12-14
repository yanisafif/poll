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
        public Task RegisterAsync(RegisterViewModel model);

        public Task<bool> Authenticated(LoginViewModel model);
        public Task Logout();


    }
}
