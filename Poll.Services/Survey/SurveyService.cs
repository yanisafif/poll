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

        public async Task AddSurveyAsync(AddSurveyViewModel surveyModel)
        {
            if (surveyModel is null)
                throw new ArgumentNullException(nameof(surveyModel));

            if (String.IsNullOrWhiteSpace(surveyModel.Name) || surveyModel.Choises.Count < 2)
                throw new ArgumentException(nameof(surveyModel));

            IEnumerable<Choice> choises = surveyModel.Choises.Select((c) => new Choice()
            {
                Name = c
            });

            Survey survey = new Survey()
            {
                CreationDate = DateTime.Now,
                Description = surveyModel.Description,
                Choices = choises.ToList(),
                Name = surveyModel.Name,
                IsActive = true,
                MultipleChoices = surveyModel.IsMultipleChoises
            };

            await this._surveyRepo.AddSurveyAsync(survey);
        }
    }
}