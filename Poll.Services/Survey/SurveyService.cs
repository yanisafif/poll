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
        private readonly IVoteRepository _voteRepo;
        private readonly IUsersService _userService;

        private readonly ILogger<SurveyService> _logger;

        public SurveyService(
            ISurveyRepository surveyRepo, 
            IVoteRepository voteRepo,
            IUsersService usersService, 
            ILogger<SurveyService> logger
        )
        {
            this._surveyRepo = surveyRepo;
            this._voteRepo = voteRepo;
            this._userService = usersService;
            this._logger = logger;
        }

        public async Task<SurveyListViewModel> GetList()
        {
            List<Survey> surveys = await this._surveyRepo.GetListAsync();

            int userId = 0;

            if(this._userService.IsUserLoggedIn())
                userId = this._userService.GetUserWithClaims().Id;

            SurveyListViewModel model = new SurveyListViewModel()
            {
                ListOfSurvey = new List<SurveyViewModel>(), 
                UserIsLoggedIn = userId != 0
            };

            if(surveys is null || surveys.Count == 0)
                return model;

            model.ListOfSurvey = surveys.Select((a) => new SurveyViewModel()
                {
                    PollName = a.Name, 
                    Username = a.User.Pseudo, 
                    CreationDate = a.CreationDate.ToShortDateString(), 
                    IsActive =  a.IsActive, 
                    Description = a.Description ?? "", 
                    Guid = a.Guid,
                    IsCurrentUser = a.User.Id == userId, 
                    UserDidVote = userId == 0 ? false : this._surveyRepo.DidUserVoteSurvey(a.Id, userId)
                }
            );

            return model;
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
                User = this._userService.GetUserWithClaims()
            };

            await this._surveyRepo.AddSurveyAsync(survey);
        }

        public async Task DeactivateAsync(string guid)
        {
            if(String.IsNullOrWhiteSpace(guid))
                throw new ArgumentNullException(nameof(guid));

            Survey survey = await this._surveyRepo.GetAsync(guid); 

            if(survey is null)
                throw new ArgumentException(nameof(guid));

            User user = this._userService.GetUserWithClaims();

            if(survey.User.Id != user.Id)
                throw new UserNotCorrespondingException();

            survey.IsActive = false; 

            await this._surveyRepo.Update(survey);
            await this._surveyRepo.Update(survey);
        }

        public List<ResultViewModel> GetResult(string guid)
        {
            var idSurvey = _surveyRepo.GetIdSurvey(guid);

            var choices = _surveyRepo.GetChoicesAsync(idSurvey);

            if (choices is null)return null;

            List<ResultViewModel> choiceModel = new List<ResultViewModel>();

            foreach(var choice in choices)
            {
                ResultViewModel objcvm = new ResultViewModel();
                objcvm.IdChoice = choice.Id;
                objcvm.NameChoice = choice.Name;
                objcvm.vote = _surveyRepo.GetVotesByChoices(choice.Id);

                choiceModel.Add(objcvm);

            }

            return choiceModel;
        }
    }

    [System.Serializable]
    public class UserNotCorrespondingException : System.Exception
    {
        public UserNotCorrespondingException(string message = "Le créateur du sondage ne correspond pas à l'utilisateur courant.") : base(message) { }
        public UserNotCorrespondingException(string message, System.Exception inner) : base(message, inner) { }
        protected UserNotCorrespondingException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
