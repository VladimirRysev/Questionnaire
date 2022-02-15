using Questionnaire.Core.Models;

namespace Questionnaire.Services.Temp;

public interface ITempQuestionnaireService
{
    void SaveTempQuestionnaire(QuestionnaireModel model);
    QuestionnaireModel GetTempQuestionnaire();
    void NewQuestionnaire();
}