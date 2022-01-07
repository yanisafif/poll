using Poll.Data.Model;
using Poll.Services.ViewModel;
using System.Threading.Tasks;

namespace Poll.Services
{
    public interface IUsersService
    {
        Task<bool> RegisterAsync(RegisterViewModel model);

        Task<bool> AuthenticatedAsync(LoginViewModel model);
        User GetUserWithClaims();
        Task LogoutAsync();
        bool IsUserLoggedIn();
    }
}
