using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Poll.Services.ViewModel
{
    public class SurveyListViewModel
    {
        public bool UserIsLoggedIn { get; set; }
        public IEnumerable<SurveyViewModel> ListOfSurvey { get; set; }
    }

    public class SurveyViewModel
    {
        public string PollName { get; set; }
        public string Username { get; set; }
        public string CreationDate { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string Guid { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}