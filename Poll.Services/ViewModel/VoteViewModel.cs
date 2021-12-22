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
        public IEnumerable<ChoiceViewModel> Choices { get; set; }
        public int Choice { get; set; }
        public string Guid { get; set; }
    }

    public class ChoiceViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }   
}