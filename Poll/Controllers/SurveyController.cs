using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Poll.Services;
using Poll.Services.ViewModel;

namespace Poll.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ILogger<SurveyController> _logger;
        private readonly ISurveyService _surveyService;
        private readonly IVoteService _voteService;

        public SurveyController(ILogger<SurveyController> logger, ISurveyService surveyService, IVoteService voteService)
        {
            _logger = logger;
            _surveyService = surveyService;
            _voteService = voteService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<SurveyViewModel> models = await this._surveyService.GetList();

            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddSurveyViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);
            
           await this._surveyService.AddSurveyAsync(model);

            return Redirect("/Survey");
        }

        [HttpGet]
        public async Task<IActionResult> Vote([FromRoute] string guid)
        {
            VoteViewModel model = await this._surveyService.GetChoicesAsync(guid);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Vote([FromRoute] string guid, VoteViewModel a)
        {
            await this._voteService.AddVote(guid, a);

            return Redirect("/Survey");
        }

        public async Task<IActionResult> Deactivate([FromRoute] string guid)
        {
            await this._surveyService.DeactivateAsync(guid);

            return Redirect("/Survey");
        }
    }
}
