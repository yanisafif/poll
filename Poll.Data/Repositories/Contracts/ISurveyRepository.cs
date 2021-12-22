using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poll.Data.Model;

namespace Poll.Data.Repositories
{
    public interface ISurveyRepository
    {
        Task<List<Survey>> GetListAsync(); 

        Task<Survey> GetAsync(int id);

        Task<Survey> GetAsync(string guid);

        Task AddSurveyAsync(Survey survey);

        Task<User> GetUserTest();

        Task<bool> IsGuidUsed(string guid);

        Task AddVoteAsync(Vote vote);
    }
}
