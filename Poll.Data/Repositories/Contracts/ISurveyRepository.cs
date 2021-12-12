using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poll.Data.Model;

namespace Poll.Data.Repositories
{
    public interface ISurveyRepository
    {
        Task<List<Survey>> GetListAsync(); 

        Task<Survey> GetAsync(int id);

    }
}
