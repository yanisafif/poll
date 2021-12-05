using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Poll.Data.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Pseudo { get; set; }
        public List<Survey> Surveys { get; set; }
        public List<Vote> Votes { get; set; }
    }
}
