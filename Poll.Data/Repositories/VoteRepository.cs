using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Poll.Data.Model;

namespace Poll.Data.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly AppDbContext _dbContext;
        public VoteRepository(AppDbContext appDbContext)
        {
            this._dbContext = appDbContext;
        }

        public bool DidUserVote(int userId, int choiceId)
        {
            return this._dbContext.Votes.Any(f => f.Choice.Id == choiceId && f.User.Id == userId);
        }
        public Vote GetVote(int userId, int choiceId)
        {
            return this._dbContext.Votes.First(f => f.User.Id == userId && f.Choice.Id == choiceId);
        }
        public int GetNumberVoter(int surveyId)
        {
            return this._dbContext.Votes.FromSqlRaw(
                "SELECT DISTINCT UserId FROM Votes WHERE ChoiceId IN (SELECT Choices.Id FROM Surveys INNER JOIN Choices ON Choices.SurveyId = Surveys.Id WHERE Surveys.Id = {0})",
                surveyId
            ).Count();
        }
        public async Task AddVoteAsync(Vote vote)
        {
            if(vote is null)
                throw new ArgumentNullException(nameof(vote)); 
            
            await this._dbContext.Votes.AddAsync(vote);
            await this._dbContext.SaveChangesAsync();
        }
        public async Task AddVotesAsync(Vote[] votes)
        {
            if(votes is null)
                throw new ArgumentNullException(nameof(votes)); 
            
            await this._dbContext.Votes.AddRangeAsync(votes);
            await this._dbContext.SaveChangesAsync();
        }

        public async Task DeleteVotesAsync(Vote[] votes)
        {
            if(votes is null)
                throw new ArgumentNullException(nameof(votes)); 
            
            this._dbContext.Votes.RemoveRange(votes);
            await this._dbContext.SaveChangesAsync();
        }

    }

}