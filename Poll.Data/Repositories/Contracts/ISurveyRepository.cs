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
        IEnumerable<Survey> GetList(int currentUserId);

        Task<Survey> GetAsync(int id);

        Task<Survey> GetAsync(string guid, GuidType guidType);

        Task AddSurveyAsync(Survey survey);

        Task Update(Survey survey);

        bool DidUserVoteSurvey(int surveyId, int userId);

        Task<List<Choice>> GetChoicesAsync(int surveyId);

        int GetVotesByChoices(int choiceId);

    }
}
