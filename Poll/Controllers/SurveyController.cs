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

        public SurveyController(ILogger<SurveyController> logger, ISurveyService surveyService)
        {
            _logger = logger;
            _surveyService = surveyService;
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
        public async Task<IActionResult> Vote()
        {
            return View();
        }
    }
}
