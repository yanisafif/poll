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
            private readonly ILogger<ISurveyService> _logger;

            public VoteService(
                ISurveyRepository surveyRepo, 
                IVoteRepository voteRepo,
                ILogger<ISurveyService> logger
            )
            {
                this._surveyRepo = surveyRepo;
                this._voteRepo = voteRepo;
                this._logger = logger;
            }

            public async Task AddVote(string guid, VoteViewModel model)
            {
                if(String.IsNullOrWhiteSpace(guid))
                    throw new ArgumentNullException(nameof(guid));
                if(model.Choices is null || model.Choices.Count == 0)
                    throw new ArgumentException(nameof(model.Choices));

                Survey survey = await this._surveyRepo.GetAsync(guid);

                if(!survey.IsActive) 
                    throw new Exception("Le sondage n'est plus actif.");

                User user = await this._surveyRepo.GetUserTest(); 

                List<Vote> votesAdd = new List<Vote>();
                List<Vote> votesDelete = new List<Vote>();
                foreach (ChoiceViewModel item in model.Choices)
                {
                    if(item.Selected && !item.SelectedBefore)
                    {
                        votesAdd.Add(this.CreateVote(survey, user, item.Id));
                    }
                    if(!item.Selected && item.SelectedBefore) 
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
                    throw new Exception("Ce sondage ne conteint pas ce vote la");

                return new Vote()
                {
                    CreationDate = DateTime.Now, 
                    Choice = choice, 
                    User = user
                };
            }
        }
    }