using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poll.Services.ViewModel
{
    public class LinkViewModel
    {
        public string Name { get; set; }
        public string GuidResult { get; set; }
        public string GuidDeactivate { get; set; }
        public string GuidVote { get; set; }


        public string GuidLink { get; set; } // Will be posted

        [DataType(DataType.EmailAddress)]
        public List<string> Emails { get; set; } // Will be posted
    }
}
