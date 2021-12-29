using Poll.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poll.Data.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext _dbContext;
        public UsersRepository(AppDbContext db)
        {
            _dbContext = db;
        }

        public bool AnyEmailOrPseudo(string email, string pseudo)
        {

            if(_dbContext.Users.Any(x => x.Email == email) || _dbContext.Users.Any(x => x.Pseudo == pseudo))
                return false;

            return true;
        }

        public async Task AddUserAsync(User user)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

        }

        public User GetUserByEmail(string email)
        {
            return _dbContext.Users.FirstOrDefault(x => x.Email == email);
        }

    }
}
