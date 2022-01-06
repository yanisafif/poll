using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Poll.Services.ViewModel;
using Poll.Data.Repositories;
using Poll.Data.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Poll.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepo;
        private readonly IVoteRepository _voteRepo;
        private readonly IUsersService _userService;
        private readonly IConfiguration _configuration;

        private readonly ILogger<SurveyService> _logger;

        public SurveyService(
            ISurveyRepository surveyRepo, 
            IVoteRepository voteRepo,
            IUsersService usersService, 
            ILogger<SurveyService> logger, 
            IConfiguration configuration
        )
        {
            this._surveyRepo = surveyRepo;
            this._voteRepo = voteRepo;
            this._userService = usersService;
            this._logger = logger;
            this._configuration = configuration;
        }

        public IEnumerable<SurveyViewModel> GetList()
        {
            int userId = 0;

            if(this._userService.IsUserLoggedIn())
                userId = this._userService.GetUserWithClaims().Id;

            List<Survey> surveys = this._surveyRepo.GetList(userId).ToList();

            if(surveys is null)
                return new List<SurveyViewModel>();

            IEnumerable<SurveyViewModel> model = surveys.Select((a) => new SurveyViewModel()
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
                IsPrivate = surveyModel.IsPrivate,
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
        }

        public async Task<List<ResultViewModel>> GetResult(string guidResult)
        {
            var idSurvey = (await _surveyRepo.GetAsync(guidResult, GuidType.Result)).Id;

            var choices = await _surveyRepo.GetChoicesAsync(idSurvey);

            if (choices is null || choices.Count == 0) return null;

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
                GuidLink = survey.GuidLink,
                Name = survey.Name
            };
        }

        public async Task<string> GetResultGuidFromVoteGuid(string voteGuid) 
        {
            return (await this._surveyRepo.GetAsync(voteGuid, GuidType.Vote)).GuidResult;
        }

        public async Task SendEmailInvitation(LinkViewModel model)
        {
            if(model is null || String.IsNullOrWhiteSpace(model.GuidLink))
                throw new ArgumentNullException(nameof(model));

            Survey survey = await this._surveyRepo.GetAsync(model.GuidLink, GuidType.Link);
            if(survey is null)
                throw new ArgumentException(nameof(model));

            User user = this._userService.GetUserWithClaims();
            if(user.Id != survey.User.Id)
                throw new UserNotCorrespondingException();

            string link = this._configuration["WebSiteName"];
            StringBuilder sbBody = new StringBuilder();
            sbBody.Append("<p>Bonjour,</p>");
            sbBody.Append($"<p>Vous avez été invité(e) à voter au sondage <strong>{survey.Name}</strong> par <strong>{survey.User.Pseudo}.</p>");
            sbBody.Append($"<p><a href=\"{link}/Survey/Vote/{survey.GuidVote}\">Cliquez ici pour voter</a>, ");
            sbBody.Append($"Ou <a href=\"{link}/Survey/Result/{survey.GuidResult}\">ici pour voir les résultats</a></p>");
            sbBody.Append($"<p>Ce message vous à été envoyé(e) par <a href=\"{link}\">{link}</a></p>");

            IConfigurationSection emailSettings = this._configuration.GetSection("EmailSettings"); 
            
            SmtpClient smtpClient = new SmtpClient(emailSettings["Host"])
            {
                Port = Convert.ToInt32(emailSettings["Port"]),
                Credentials = new NetworkCredential(emailSettings["Email"], emailSettings["Password"]),
                EnableSsl = true,
            };

            MailMessage msg = new MailMessage()
            {
                IsBodyHtml = true, 
                Body = sbBody.ToString(), 
                From = new MailAddress(emailSettings["Email"]), 
                Subject = $"Invitation au sondage {survey.Name}", 
            };
            msg.Bcc.Add(String.Join(',', model.Emails));
            await smtpClient.SendMailAsync(msg);
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
