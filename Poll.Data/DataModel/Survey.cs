using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Poll.Data.Model
{
    public class Survey
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool MultipleChoices { get; set; }
        public List<Choice> Choices { get; set; }
        public User User { get; set; }
    }
}
