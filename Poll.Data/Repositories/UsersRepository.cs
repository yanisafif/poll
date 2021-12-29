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
        private readonly AppDbContext _context;
        public UsersRepository(AppDbContext db)
        {
            _context = db;
        }

        public bool AnyEmailOrPseudo(string email, string pseudo)
        {

            if(_context.Users.Any(x => x.Email == email) || _context.Users.Any(x => x.Pseudo == pseudo))
                return false;

            return true;
        }

        public async Task AddUserAsync(User user)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(x => x.Email == email);
        }
    }
}
