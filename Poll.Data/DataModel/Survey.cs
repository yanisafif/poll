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

        [StringLength(220)]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool MultipleChoices { get; set; }
        public bool IsPrivate { get; set; }
        public List<Choice> Choices { get; set; }
        public User User { get; set; }
        public DateTime CreationDate { get; set; }

        public string GuidVote { get; set; }
        public string GuidResult { get; set; }
        public string GuidDeactivate { get; set; }
        public string GuidLink { get; set; }
    }
}
