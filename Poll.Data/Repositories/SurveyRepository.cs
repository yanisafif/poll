using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Poll.Data.Model;
using Microsoft.Extensions.Logging;
using Poll.Data;
using System.Reflection;

namespace Poll.Data.Repositories
{
    public enum GuidType
    {
        Vote, 
        Result, 
        Deactivate, 
        Link
    }

    public class SurveyRepository : ISurveyRepository
    {
        private readonly AppDbContext _dbContext;
        public SurveyRepository(
            AppDbContext appDbContext
        )
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

        public Task<Survey> GetAsync(string guid, GuidType guidType)
        {
            var queryable = this._dbContext.Surveys
            .Include(a => a.User)
            .Include(a => a.Choices);

            switch(guidType) {
                case GuidType.Vote:  
                    return queryable.FirstOrDefaultAsync(m => m.GuidVote == guid);
                case GuidType.Result:
                    return queryable.FirstOrDefaultAsync(m => m.GuidResult == guid); 
                case GuidType.Deactivate:
                    return queryable.FirstOrDefaultAsync(m => m.GuidDeactivate == guid);
                case GuidType.Link:
                    return queryable.FirstOrDefaultAsync(m => m.GuidLink == guid);
                default: 
                    throw new ArgumentException(nameof(guidType));
            }
        }

        public IEnumerable<Survey> GetList(int currentUserId)
        {
            var queryable = this._dbContext.Surveys
            .Include(a => a.User)
            .Include(a => a.Choices)
            .OrderBy(f => f.CreationDate); 

            if(currentUserId > 0)
                return queryable.Where(f => !f.IsPrivate || f.User.Id == currentUserId);
            else 
                return queryable.Where(f => !f.IsPrivate);


        }

        public async Task AddSurveyAsync(Survey survey)
        {
            if (survey is null)
                throw new ArgumentNullException(nameof(survey));

            await this._dbContext.Surveys.AddAsync(survey);
            await this._dbContext.SaveChangesAsync();
        }
        
        public async Task Update(Survey survey)
        {
            if(survey is null)
                throw new ArgumentNullException(nameof(survey)); 

            this._dbContext.Surveys.Update(survey);
            await this._dbContext.SaveChangesAsync();
        }
        
        public bool DidUserVoteSurvey(int surveyId, int userId)
        {
            return this._dbContext.Votes.Any(e => e.Choice.Survey.Id == surveyId && e.User.Id == userId);

        }

        public async Task<List<Choice>> GetChoicesAsync(int surveyId)
        {
            return await this._dbContext.Choices.Where(e => e.Survey.Id == surveyId).ToListAsync();
        }

        public async Task<int> GetVotesByChoicesAsync(int choiceId)
        {
            var vote = await _dbContext.Votes.Where(e => e.Choice.Id == choiceId).Select(e => e.Id).CountAsync();

            if (vote > 0)
            {
                return vote;
            }
            else
            {
                return 0;
            }
        }

        public async Task<Survey> GetSurveyByGuidAsync(string guid)
        {
            return await _dbContext.Surveys.FirstOrDefaultAsync(f => f.GuidResult == guid);
        }
    }
}