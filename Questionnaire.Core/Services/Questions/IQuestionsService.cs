using Questionnaire.Core.Models;

namespace Questionnaire.Core.Services.Questions;

public interface IQuestionsService
{
    ServiceResult<List<string>> GetQuestionsList();
}