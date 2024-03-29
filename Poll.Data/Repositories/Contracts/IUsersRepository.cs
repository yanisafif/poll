﻿using Poll.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poll.Data.Repositories
{
    public interface IUsersRepository
    {
        bool AnyEmailOrPseudo(string email, string pseudo);
        Task AddUserAsync(User user);
        User GetUserByEmail(string email);
    }
}
