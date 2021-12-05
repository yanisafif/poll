using System;
using Microsoft.EntityFrameworkCore;
using Poll.Data.Model;

namespace Poll.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vote> Votes { get; set; }
    }
}
