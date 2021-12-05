using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Poll.Data.DataModel
{
    public class Choice
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }
        public Poll Poll { get; set; }
        List<Vote> Votes { get; set; }
    }
}