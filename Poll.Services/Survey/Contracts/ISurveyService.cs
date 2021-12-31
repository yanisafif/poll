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
        Task<IEnumerable<SurveyViewModel>> GetList();

        Task AddSurveyAsync(AddSurveyViewModel surveyModel);

        Task DeactivateAsync(string guid);
        List<ResultViewModel> GetResult(string guid);
    }
}
