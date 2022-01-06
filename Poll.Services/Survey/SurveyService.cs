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
                    GuidDeactivate = a.GuidDeactivate,
                    GuidResult = a.GuidResult,
                    GuidVote = a.GuidVote,
                    IsCurrentUser = a.User.Id == userId, 
                    UserDidVote = userId == 0 ? false : this._surveyRepo.DidUserVoteSurvey(a.Id, userId)
                }
            );

            return model;
        }

        public async Task<string> AddSurveyAsync(AddSurveyViewModel surveyModel)
        {
            if (surveyModel is null)
                throw new ArgumentNullException(nameof(surveyModel), "Le model est vide");
            
            if(surveyModel.Choices.Count < 2)
                throw new NotEnoughChoicesException();

            List<Choice> choices = new List<Choice>();
            
            foreach (string item in surveyModel.Choices)
            {
                if(String.IsNullOrWhiteSpace(item))
                    continue;

                choices.Add(new Choice() { Name = item });
            }

            if(choices.Count < 2)
                throw new NotEnoughChoicesException();

            Survey survey = new Survey()
            {
                CreationDate = DateTime.Now,
                Description = surveyModel.Description,
                Choices = choices,
                Name = surveyModel.Name,
                IsActive = true,
                MultipleChoices = surveyModel.IsMultipleChoices, 
                GuidDeactivate = Guid.NewGuid().ToString(),
                GuidLink = Guid.NewGuid().ToString(),
                GuidResult = Guid.NewGuid().ToString(),
                GuidVote = Guid.NewGuid().ToString(),
                User = this._userService.GetUserWithClaims()
            };

            await this._surveyRepo.AddSurveyAsync(survey);

            return survey.GuidLink;
        }


        public async Task DeactivateAsync(string deactivateGuid)
        {
            if(String.IsNullOrWhiteSpace(deactivateGuid))
                throw new ArgumentNullException(nameof(deactivateGuid));

            Survey survey = await this._surveyRepo.GetAsync(deactivateGuid, GuidType.Deactivate); 

            if(survey is null)
                throw new ArgumentException(nameof(deactivateGuid));

            User user = this._userService.GetUserWithClaims();

            if(survey.User.Id != user.Id)
                throw new UserNotCorrespondingException();

            survey.IsActive = false; 

            await this._surveyRepo.Update(survey);
            await this._surveyRepo.Update(survey);
        }

        public async Task<List<ResultViewModel>> GetResult(int idSurvey)
        {
            var choices = await _surveyRepo.GetChoicesAsync(idSurvey);

            if (choices is null || choices.Count == 0) return null;

            List<ResultViewModel> choiceModel = new List<ResultViewModel>();

            foreach(var choice in choices)
            {
                ResultViewModel objcvm = new ResultViewModel();
                objcvm.IdChoice = choice.Id;
                objcvm.NameChoice = choice.Name;
                objcvm.vote = await _surveyRepo.GetVotesByChoicesAsync(choice.Id);

                choiceModel.Add(objcvm);

            }

            return choiceModel;
        }

        public async Task<Survey> GetSurveyAsync(string guidResult)
        {
            return await _surveyRepo.GetAsync(guidResult, GuidType.Result);
        }

        public int GetNumberVote(int idSurvey)
        {
            return _voteRepo.GetNumberVoter(idSurvey);
        }

        public async Task<LinkViewModel> GetLinkViewModelAsync(string linkGuid)
        {
            Survey survey =  await this._surveyRepo.GetAsync(linkGuid, GuidType.Link);

            User user = this._userService.GetUserWithClaims();

            if(user.Id != survey.User.Id)
                throw new UserNotCorrespondingException();

            return new LinkViewModel()
            {
                GuidDeactivate = survey.GuidDeactivate, 
                GuidResult = survey.GuidResult, 
                GuidVote = survey.GuidVote, 
                Name = survey.Name
            };
        }

        public async Task<string> GetResultGuidFromVoteGuid(string voteGuid) 
        {
            return (await this._surveyRepo.GetAsync(voteGuid, GuidType.Vote)).GuidResult;
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
    [System.Serializable]
    public class NotEnoughChoicesException : System.Exception
    {
        public NotEnoughChoicesException(string message = "Il n'y a pas assez de choix.") : base(message) { }
        public NotEnoughChoicesException(string message, System.Exception inner) : base(message, inner) { }
        protected NotEnoughChoicesException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
