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
        Task<IEnumerable<SurveyViewModel>> GetListAsync();

        Task<string> AddSurveyAsync(AddSurveyViewModel surveyModel);

        Task DeactivateAsync(string guid);

        Task<List<ResultViewModel>> GetResultAsync(int idSurvey);

        Task<Survey> GetSurveyAsync(string guidResult);

        Task<string> GetResultGuidFromVoteGuidAsync(string voteGuid);
        int GetNumberVote(int idSurvey);

        Task<LinkViewModel> GetLinkViewModelAsync(string linkGuid);

        Task SendEmailInvitationAsync(LinkViewModel model);

        bool IsSurveyActive(Survey survey);

        string GetDeactivateDate(Survey survey);

        Task DeleteAsync(string deactivateGuid);
    }
}
