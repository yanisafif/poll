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
            .FirstOrDefaultAsync(m => m.Id == id);
        }

        public Task<Survey> GetAsync(string guid)
        {
            return this._dbContext.Surveys
            .Include(a => a.User)
            .Include(a => a.Choices)
            .FirstOrDefaultAsync(m => m.Guid == guid);
        }

        public Task<List<Survey>> GetListAsync()
        {
            return this._dbContext.Surveys
            .Include(a => a.User)
            .Include(a => a.Choices)
            .OrderBy(f => f.CreationDate)
            .ToListAsync();
        }

        public async Task AddSurveyAsync(Survey survey)
        {
            if (survey is null)
                throw new ArgumentNullException(nameof(survey));

            await this._dbContext.Surveys.AddAsync(survey);
            await this._dbContext.SaveChangesAsync();
        }

        public Task<bool> IsGuidUsed(string guid)
        {
            return this._dbContext.Surveys.AnyAsync(s => s.Guid == guid);
        }
        
        public async Task Update(Survey survey)
        {
            if(survey is null)
                throw new ArgumentNullException(nameof(survey)); 

            this._dbContext.Surveys.Update(survey);
            await this._dbContext.SaveChangesAsync();
        }


        public Task<User> GetUserTest()
        {
            return this._dbContext.Users.FirstOrDefaultAsync(u => u.Id == 2);
        }

        public List<Choice> GetChoicesAsync(int surveyId)
        {
            var choice = this._dbContext.Choices.FromSqlRaw(
                 "SELECT * FROM choices WHERE SurveyId = {0}", surveyId).ToList();
            if (choice.Count == 0)  return null; 
            return choice;
        }

        public int GetVotesByChoices(int choiceId)
        {
            var vote = this._dbContext.Votes.FromSqlRaw(
                 "SELECT Id FROM votes WHERE choiceId = {0}",
                 choiceId).Count();

            if (vote > 0)
            {
                return vote;
            }
            else
            {
                return 0;
            }
        }
    }
}