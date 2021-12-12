using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Poll.Services.ViewModel;
using Poll.Data.Repositories;
using Poll.Data.Model;

namespace Poll.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepo;

        public SurveyService(ISurveyRepository surveyRepo)
        {
            this._surveyRepo = surveyRepo;
        }

        public async Task<IEnumerable<SurveyPreviewViewModel>> GetListPreviewAsync()
        {
            List<Survey> surveys = await this._surveyRepo.GetListAsync();

            if(surveys is null || surveys.Count == 0)
                return new List<SurveyPreviewViewModel>();

            IEnumerable<SurveyPreviewViewModel> models = surveys.Select((a) => 
                new SurveyPreviewViewModel()
                {
                    PollName = a.Name, 
                    UserName = a.User.Pseudo, 
                    CreationDate = a.CreationDate.ToShortDateString(), 
                    IsActive =  a.IsActive
                }
            );

            return models ?? new List<SurveyPreviewViewModel>();
        }
    }
}