using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Poll.Data.Model
{
    public class Vote
    {
        [Key]
        public int Id { get; set; }
        
        public User User { get; set; }
        public Choice Choice { get; set; }
    }
}
