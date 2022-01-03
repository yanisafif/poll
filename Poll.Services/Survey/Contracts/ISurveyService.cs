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
        Task<SurveyListViewModel> GetList();

        Task<string> AddSurveyAsync(AddSurveyViewModel surveyModel);

        Task DeactivateAsync(string guid);

        Task<List<ResultViewModel>> GetResult(string guidResult);

        Task<string> GetResultGuidFromVoteGuid(string voteGuid);

        Task<LinkViewModel> GetLinkViewModelAsync(string linkGuid);
    }
}
