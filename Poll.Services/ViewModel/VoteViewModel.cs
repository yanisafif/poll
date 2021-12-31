using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Poll.Services.ViewModel
{
    public class VoteViewModel
    {
        public string PollName { get; set; }
        public int NumberOfVoter { get; set; }
        public string Description { get; set; }
        public bool IsMultipleChoice { get; set; }
        public List<ChoiceViewModel> Choices { get; set; }
        public string Guid { get; set; }
    }

    public class ChoiceViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool Selected { get; set; }
        public bool SelectedBefore { get; set; }
    }   
}