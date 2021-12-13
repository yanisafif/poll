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
            IEnumerable<SurveyPreviewViewModel> models = await this._surveyService.GetListPreviewAsync();

            return View(models);
        }
    }
}
