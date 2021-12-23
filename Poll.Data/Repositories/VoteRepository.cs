using System;
using System.Threading.Tasks;
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
        public async Task AddVoteAsync(Vote vote)
        {
            if(vote is null)
                throw new ArgumentNullException(nameof(vote)); 
            
            await this._dbContext.Votes.AddAsync(vote);
            await this._dbContext.SaveChangesAsync();
        }
        public async Task AddVotesAsync(Vote[] vote)
        {
            if(vote is null)
                throw new ArgumentNullException(nameof(vote)); 
            
            await this._dbContext.Votes.AddRangeAsync(vote);
            await this._dbContext.SaveChangesAsync();
        }
    }

}