using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poll.Data.Model;

namespace Poll.Data.Repositories
{
    public interface IVoteRepository
    {
        bool DidUserVote(int userId, int choiceId);
        Vote GetVote(int userId, int choiceId);

        Task AddVoteAsync(Vote vote);

        Task AddVotesAsync(Vote[] vote);

        Task DeleteVotesAsync(Vote[] votes);
    }
}
