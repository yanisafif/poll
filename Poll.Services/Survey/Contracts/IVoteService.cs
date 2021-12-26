using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poll.Services.ViewModel;
namespace Poll.Services
{
    public interface IVoteService
    {
        Task<VoteViewModel> GetVoteViewModelAsync(string guid);

        Task AddVote(string guid, VoteViewModel model);
    }
}
