using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Poll.Data.Model;
using Poll.Data;

namespace Poll.Data.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly AppDbContext _dbContext;
        public SurveyRepository(AppDbContext appDbContext)
        {
            this._dbContext = appDbContext;
        }
        public Task<Survey> GetAsync(int id)
        {
            return this._dbContext.Surveys
            .Include(a => a.User)
            .Include(a => a.Choices)
            .FirstOrDefaultAsync();
        }

        public Task<List<Survey>> GetListAsync()
        {
            return this._dbContext.Surveys
            .Include(a => a.User)
            .Include(a => a.Choices)
            .ToListAsync();
        }

        public async Task AddSurveyAsync(Survey survey)
        {
            if (survey is null)
                throw new ArgumentNullException(nameof(survey));

            await this._dbContext.Surveys.AddAsync(survey);
            await this._dbContext.SaveChangesAsync();
        }

        public Task<User> GetUserTest()
        {
            return this._dbContext.Users.FirstOrDefaultAsync(u => u.Id == 2);
        }
    }
}