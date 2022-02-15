using Questionnaire.Core.Models;

namespace Questionnaire.Services.Temp;

public class TempQuestionnaireService : ITempQuestionnaireService
{
    
    private QuestionnaireModel _questionnaire;

    public void SaveTempQuestionnaire(QuestionnaireModel model)
    {
        _questionnaire = model;
    }

    public QuestionnaireModel GetTempQuestionnaire()
    {
        return _questionnaire;
    }

    public void NewQuestionnaire()
    {
        _questionnaire = new QuestionnaireModel();
    }
}