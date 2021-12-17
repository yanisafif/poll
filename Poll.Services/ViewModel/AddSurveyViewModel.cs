using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poll.Services.ViewModel
{
    public class AddSurveyViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Choises { get; set; }
        public bool IsMultipleChoises { get; set; }

    }
}
