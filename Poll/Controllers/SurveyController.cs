﻿using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Poll.Services;
using Poll.Services.ViewModel;
using Microsoft.AspNetCore.Authorization;

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
            SurveyListViewModel model = await this._surveyService.GetList();

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(AddSurveyViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);
            
            try
            {
                await this._surveyService.AddSurveyAsync(model);
            }
            catch(Exception e) when (
                e is ArgumentException ||
                e is ArgumentNullException
            )
            {
                ModelState.AddModelError("model", "Une erreur s'est produite");
                return View(model);
            }

            return Redirect("/Survey");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Vote([FromRoute] string guid)
        {
            
            VoteViewModel model;
            try
            {
                model = await this._voteService.GetVoteViewModelAsync(guid);
            }
            catch(SurveyDeactivatedException)
            {
                return Redirect($"/Survey/Result/{guid}");
            }
            catch(Exception e) when (
                e is ArgumentException ||
                e is ArgumentNullException
            )
            {
                return Redirect("/Survey");
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Vote([FromRoute] string guid, VoteViewModel a)
        {
            await this._voteService.AddVote(guid, a);

            return Redirect("/Survey");
        }

        [Authorize]
        public async Task<IActionResult> Deactivate([FromRoute] string guid)
        {
            await this._surveyService.DeactivateAsync(guid);

            return Redirect("/Survey");
        }
    }
}
