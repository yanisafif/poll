using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Poll.Services.ViewModel
{
    public class SurveyViewModel
    {
        public string PollName { get; set; }
        public string Username { get; set; }
        public string CreationDate { get; set; }
        public bool IsActive { get; set; }
        public string DeactivateDate { get; set; }
        public string Description { get; set; }
        public string GuidResult { get; set; }
        public string GuidVote { get; set; }
        public string GuidDeactivate { get; set; }
        public bool IsCurrentUser { get; set; }
        public bool UserDidVote { get; set; }
    }
}