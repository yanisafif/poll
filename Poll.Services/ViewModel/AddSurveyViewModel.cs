using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Poll.Services.ViewModel
{
    public class AddSurveyViewModel
    {
        [Required(ErrorMessage = "Le nom est requis")]
        [Display(Name = "Nom")]
        [StringLength(50)]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [StringLength(220)]
        public string Description { get; set; }

        public List<string> Choices { get; set; }

        [Display(Name = "Plusieurs choix sont possible")]
        public bool IsMultipleChoices { get; set; }

    }
}
