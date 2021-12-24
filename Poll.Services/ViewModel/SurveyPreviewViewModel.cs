using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Poll.Services.ViewModel
{
    public class SurveyPreviewViewModel
    {
        public string PollName { get; set; }
        public string UserName { get; set; }
        public string CreationDate { get; set; }
        public bool IsActive { get; set; }
        
    }
}