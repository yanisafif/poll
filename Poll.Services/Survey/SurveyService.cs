    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Poll.Services.ViewModel;
    using Poll.Data.Repositories;
    using Poll.Data.Model;
    using Microsoft.Extensions.Logging;
    namespace Poll.Services
    {
        public class SurveyService : ISurveyService
        {
            private readonly ISurveyRepository _surveyRepo;
            private readonly ILogger<ISurveyService> _logger;

            public SurveyService(ISurveyRepository surveyRepo, ILogger<ISurveyService> logger)
            {
                this._surveyRepo = surveyRepo;
                this._logger = logger;
            }

            public async Task<IEnumerable<SurveyViewModel>> GetList()
            {
                List<Survey> surveys = await this._surveyRepo.GetListAsync();

                if(surveys is null || surveys.Count == 0)
                    return new List<SurveyViewModel>();

                IEnumerable<SurveyViewModel> models = surveys.Select((a) => new SurveyViewModel()
                    {
                        PollName = a.Name, 
                        Username = a.User.Pseudo, 
                        CreationDate = a.CreationDate.ToShortDateString(), 
                        IsActive =  a.IsActive, 
                        Description = a.Description ?? "", 
                        Guid = a.Guid
                    }
                );

                return models ?? new List<SurveyViewModel>();
            }

            public async Task AddSurveyAsync(AddSurveyViewModel surveyModel)
            {
                if (surveyModel is null)
                    throw new ArgumentNullException(nameof(surveyModel));

                if (String.IsNullOrWhiteSpace(surveyModel.Name) || surveyModel.Choices.Count < 2)
                    throw new ArgumentException(nameof(surveyModel));

                List<Choice> choices = new List<Choice>();
                
                foreach (string item in surveyModel.Choices)
                {
                    if(String.IsNullOrWhiteSpace(item))
                        continue;

                    choices.Add(new Choice() { Name = item });
                }

                string guid;
                do
                {
                    guid = Guid.NewGuid().ToString();
                }
                while(await this._surveyRepo.IsGuidUsed(guid));

                Survey survey = new Survey()
                {
                    CreationDate = DateTime.Now,
                    Description = surveyModel.Description,
                    Choices = choices,
                    Name = surveyModel.Name,
                    IsActive = true,
                    MultipleChoices = surveyModel.IsMultipleChoices, 
                    Guid = Guid.NewGuid().ToString(),
                    ///To Test 
                    User = await this._surveyRepo.GetUserTest()
                    //End To Test
                };

                await this._surveyRepo.AddSurveyAsync(survey);
            }

            public async Task<VoteViewModel> GetChoicesAsync(string guid)
            {
                if(String.IsNullOrWhiteSpace(guid))
                    throw new ArgumentNullException(nameof(guid));

                Survey survey = await this._surveyRepo.GetAsync(guid);

                if(survey is null)
                    throw new ArgumentException();
                
                IEnumerable<ChoiceViewModel> choices = survey.Choices.Select(m => new ChoiceViewModel() 
                {
                    Id = m.Id,
                    Name = m.Name
                });

                return new VoteViewModel() 
                {
                    Choices = choices, 
                    Guid = guid,
                    PollName = survey.Name
                };
            }

            public async Task AddVote(string guid, VoteViewModel a)
            {
                if(String.IsNullOrWhiteSpace(guid))
                    throw new ArgumentNullException(nameof(guid));
                if(a.Choice <= 0)
                    throw new ArgumentNullException(nameof(a.Choice));

                Survey survey = await this._surveyRepo.GetAsync(guid);

                if(!survey.IsActive) 
                    throw new Exception("Le sondage n'est plus actif.");

                Choice choice = survey.Choices.FirstOrDefault(f => f.Id == a.Choice);
                if(choice is null) 
                    throw new Exception("Ce sondage ne conteint pas ce vote la");
                
                Vote vote = new Vote()
                {
                    CreationDate = DateTime.Now, 
                    Choice = choice, 
                    User = await this._surveyRepo.GetUserTest()
                };

                await this._surveyRepo.AddVoteAsync(vote);
            } 

            public async Task DeactivateAsync(string guid)
            {
                if(String.IsNullOrWhiteSpace(guid))
                    throw new ArgumentNullException(nameof(guid));

                Survey survey = await this._surveyRepo.GetAsync(guid); 

                if(survey is null)
                    throw new ArgumentException(nameof(guid));
                
                // TO DO: Check if current user owns the survey

                survey.IsActive = false; 

                await this._surveyRepo.Update(survey);
            }
        }
    }