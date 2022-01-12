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
    public class VoteService : IVoteService
    {
        private readonly ISurveyRepository _surveyRepo;
        private readonly IVoteRepository _voteRepo;
        private readonly IUsersService _userService;
        private readonly ILogger<VoteService> _logger;
        private readonly ISurveyService _surveyService;
        public VoteService(
            ISurveyRepository surveyRepo, 
            IVoteRepository voteRepo,
            IUsersService usersService,
            ISurveyService surveyService,
            ILogger<VoteService> logger
        )
        {
            this._surveyRepo = surveyRepo;
            this._voteRepo = voteRepo;
            this._userService = usersService;
            this._surveyService = surveyService;
            this._logger = logger;
        }

        public async Task<VoteViewModel> GetVoteViewModelAsync(string voteGuid)
        {
            if(String.IsNullOrWhiteSpace(voteGuid))
                throw new ArgumentNullException(nameof(voteGuid));

            Survey survey = await this._surveyRepo.GetAsync(voteGuid, GuidType.Vote);

            this._logger.LogError("Survey null : " + (survey is null));

            if(survey is null)
                throw new ArgumentException();

            if(!this._surveyService.IsSurveyActive(survey))
                throw new SurveyDeactivatedException();

            User user = this._userService.GetUserWithClaims();
            
            IEnumerable<ChoiceViewModel> choices = survey.Choices.Select(m => 
            {
                bool isSelected = this._voteRepo.DidUserVoteChoice(user.Id, m.Id);
                return new ChoiceViewModel() 
                {
                    Id = m.Id,
                    Name = m.Name, 
                    Selected = isSelected, 
                    SelectedBefore = isSelected,
                };
            });

            return new VoteViewModel() 
            {
                Choices = choices.ToList(), 
                Guid = voteGuid,
                PollName = survey.Name, 
                IsMultipleChoice =  survey.MultipleChoices, 
                NumberOfVoter = this._voteRepo.GetNumberVoter(survey.Id), 
                Description = survey.Description, 
                DeactivateDate = this._surveyService.GetDeactivateDate(survey)
            };
        }


        public async Task AddVoteAsync(string guidVote, VoteViewModel model)
        {
            if(String.IsNullOrWhiteSpace(guidVote))
                throw new ArgumentNullException(nameof(guidVote));
            if(model.Choices is null || model.Choices.Count == 0)
                throw new ArgumentException(nameof(model.Choices));

            Survey survey = await this._surveyRepo.GetAsync(guidVote, GuidType.Vote);

            if(survey is null)
                throw new ArgumentException();

            if(!this._surveyService.IsSurveyActive(survey)) 
                throw new SurveyDeactivatedException();

            User user = this._userService.GetUserWithClaims();

            List<Vote> votesAdd = new List<Vote>();
            List<Vote> votesDelete = new List<Vote>();
            foreach (ChoiceViewModel item in model.Choices)
            {
                if(item.Selected && !item.SelectedBefore)
                {
                    votesAdd.Add(this.CreateVote(survey, user, item.Id));
                }
                else if(!item.Selected && item.SelectedBefore) 
                {
                    votesDelete.Add(this._voteRepo.GetVote(user.Id, item.Id));
                }
            }

            if(!survey.MultipleChoices && votesAdd.Count > 1)
                throw new Exception("Plusieurs choix ont été sélectionnés alors que le sondage ne permet pas les choix multiples");

            if(votesAdd.Count > 0)
                await this._voteRepo.AddVotesAsync(votesAdd.ToArray());
            if(votesDelete.Count > 0)
                await this._voteRepo.DeleteVotesAsync(votesDelete.ToArray());
        }

        private Vote CreateVote(Survey survey, User user, int choiceId)
        {
            Choice choice = survey.Choices.FirstOrDefault(f => f.Id == choiceId);

            if(choice is null) 
                throw new Exception("Ce sondage ne contient pas ce vote la");

            return new Vote()
            {
                CreationDate = DateTime.Now, 
                Choice = choice, 
                User = user
            };
        }
    }

    [System.Serializable]
    public class SurveyDeactivatedException : System.Exception
    {
        public SurveyDeactivatedException(string message = "Le sondage n'est plus actif.") : base(message) { }
        public SurveyDeactivatedException(string message, System.Exception inner) : base(message, inner) { }
        protected SurveyDeactivatedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}