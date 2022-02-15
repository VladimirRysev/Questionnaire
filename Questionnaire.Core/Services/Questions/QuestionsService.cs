using Questionnaire.Core.Models;

namespace Questionnaire.Core.Services.Questions;

public class QuestionsService : IQuestionsService
{
    private List<string> _questionsList;

    public QuestionsService()
    {
        _questionsList = new List<string>()
        {
            "Введите ФИО",
            "Введите дату рождения (Формат ДД.ММ.ГГГГ)",
            "Введите любимый язык программирования (Можно ввести только указанные варианты: PHP, JavaScript, C, C++, Java, C#, Python, Ruby)",
            "Введите опыт программирования на указанном языке (Полных лет)",
            "Введите мобильный телефон"
        };
    }

    public ServiceResult<List<string>> GetQuestionsList()
    {
        return ServiceResult<List<string>>.SuccessResult(_questionsList);
    }
}