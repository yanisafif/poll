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
                if(model.Choice <= 0 && !model.IsMultipleChoice)
                    throw new ArgumentNullException(nameof(model.Choice));

                Survey survey = await this._surveyRepo.GetAsync(guid);

                if(!survey.IsActive) 
                    throw new Exception("Le sondage n'est plus actif.");

                User user = await this._surveyRepo.GetUserTest(); 

                if(model.IsMultipleChoice)
                {
                    List<Vote> votes = new List<Vote>();
                    foreach (ChoiceViewModel item in model.Choices)
                    {
                        if(item.Selected)
                        {
                            votes.Add(this.CreateVote(survey, user, item.Id));
                        }
                    }
                    await this._voteRepo.AddVotesAsync(votes.ToArray());
                }
                else
                {
                    Vote vote = this.CreateVote(survey, user, model.Choice);
                    await this._voteRepo.AddVoteAsync(vote);
                }
                
            }

            private Vote CreateVote(Survey survey, User user, int voteId)
            {
                Choice choice = survey.Choices.FirstOrDefault(f => f.Id == voteId);

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