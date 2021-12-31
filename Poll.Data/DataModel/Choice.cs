using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Poll.Data.Model
{
    public class Choice
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        public Survey Survey { get; set; }
        public List<Vote> Votes { get; set; }
    }
}
