using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poll.Data.Model;
using Poll.Services.ViewModel;
namespace Poll.Services
{
    public interface ISurveyService
    {
        IEnumerable<SurveyViewModel> GetList();

        Task<string> AddSurveyAsync(AddSurveyViewModel surveyModel);

        Task DeactivateAsync(string guid);

        Task<List<ResultViewModel>> GetResult(int idSurvey);

        Task<Survey> GetSurveyAsync(string guidResult);

        Task<string> GetResultGuidFromVoteGuid(string voteGuid);
        int GetNumberVote(int idSurvey);

        Task<LinkViewModel> GetLinkViewModelAsync(string linkGuid);

        Task SendEmailInvitation(LinkViewModel model);
    }
}
