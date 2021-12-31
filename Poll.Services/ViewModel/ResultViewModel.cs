using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poll.Services.ViewModel
{
    public class ResultViewModel
    {
        public int IdChoice { get; set; }
        public string NameChoice { get; set; }
        public int vote { get; set; }
    }
}
