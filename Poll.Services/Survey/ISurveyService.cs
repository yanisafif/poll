using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poll.Services.ViewModel;
namespace Poll.Services
{
    public interface ISurveyService
    {
        Task<IEnumerable<SurveyViewModel>> GetList();

        Task AddSurveyAsync(AddSurveyViewModel surveyModel);

        Task<VoteViewModel> GetChoicesAsync(string guid);

        Task AddVote(string guid, VoteViewModel a);

        Task DeactivateAsync(string guid);
    }
}
