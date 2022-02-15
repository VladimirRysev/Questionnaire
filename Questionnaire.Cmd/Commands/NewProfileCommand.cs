using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Questionnaire.Common.Helpers;
using Questionnaire.Core.Models;
using Questionnaire.Core.Services.Questions;
using Questionnaire.Services;
using Questionnaire.Services.Questionnaire;
using Questionnaire.Services.Temp;

namespace Questionnaire.Commands;

public class NewProfileCommand: ICommand
{
    private readonly IQuestionsService _questionsService;
    private readonly ITempQuestionnaireService _tempQuestionnairesService;
    public string CommandName { get; }
    public string Description { get; }
    private QuestionnaireModel _model;
    private ILogger<NewProfileCommand> _logger;
    private int _currentQuestionIndex;

    private delegate void InnerCommand(string[] args);
    private Dictionary<string, InnerCommand> _innerCommands;
    private int _questionsCount;
    public NewProfileCommand(IQuestionsService questionsService, ITempQuestionnaireService tempQuestionnairesService, ILogger<NewProfileCommand> logger)
    {
        _questionsService = questionsService;
        _tempQuestionnairesService = tempQuestionnairesService;
        _logger = logger;
        CommandName = "-new_profile";
        Description = "Заполнить новую анкету";
        _innerCommands = new Dictionary<string, InnerCommand>()
        {
            {"-goto_question", GoToQuestion},
            {"-goto_prev_question", GoToPrevQuestion},
            {"-restart_profile", NewProfile},
            {"-help", InnerHelp}
        };
    }

    private void Exit(string[] args)
    {
        
    }

    private void InnerHelp(string[] args)
    {
        Help();
        Console.WriteLine("Внутренние команды:");
        Console.WriteLine("-goto_question:  Вернуться к указанному вопросу. -goto_question <Номер вопроса>");
        Console.WriteLine("-goto_prev_question:  Вернуться к предыдущему вопросу");
        Console.WriteLine("-restart_profile:   Заполнить анкету заново");
    }

    private void NewProfile(string[] args)
    {
        Execute(args);
    }

    public void Help()
    {
        Console.WriteLine($"{CommandName}: {Description}");
    }

    public void Execute(string[] args)
    {
        _model = new QuestionnaireModel();
        var questionsResult = _questionsService.GetQuestionsList();
        if (!questionsResult.Success)
        {
            Console.WriteLine(questionsResult.Message);
        }
        
        _questionsCount = questionsResult.Result?.Count??0;
        var isValid = false;
        while (!isValid)
        {
            for(_currentQuestionIndex = 0; _currentQuestionIndex < _questionsCount;)
            {
                var question = questionsResult.Result[_currentQuestionIndex];
                Console.WriteLine(question);
                var answer = Console.ReadLine();
                if (string.IsNullOrEmpty(answer))
                {
                    _logger.LogWarning($"Пустой ответ на вопрос: {question}");
                    Console.WriteLine("Введите ответ");
                    Console.WriteLine();
                    continue;
                }
                if (answer.StartsWith("-"))
                {
                    ExecuteInnerCommand(answer);
                    continue;
                }
                if (MapAnswers(GetPropertyNameFromQuestionIndex(_currentQuestionIndex), answer))
                {
                    _currentQuestionIndex++;
                }
            }

            isValid = ValidateModel(_model);
        }
        _model.Date = DateTime.Now;
        _tempQuestionnairesService.SaveTempQuestionnaire(_model);
        Console.WriteLine("Для сохранения анкеты введите -save");
    }

    private void ExecuteInnerCommand(string answer)
    {
        var args = answer?.Split(" ");
        if (args.Length > 0)
        {
            var commmand = args[0];
            if (_innerCommands.ContainsKey(commmand))
            {
                _innerCommands[commmand](args.Skip(1).ToArray());
            }
            else
            {
                Console.WriteLine("Команда не распознана");
                _logger.LogWarning($"Команда не распознана {answer}");
            }
        }
        
    }

    private void GoToQuestion(string[]? args)
    {
        if (args == null)
        {
            MessageAndLog("Не введен номер вопроса");
        }
        if (int.TryParse(args[0], out var questionNumber))
        {
            var index = questionNumber - 1;
            if (index > _questionsCount - 1 || index < 0)
            {
                MessageAndLog($"Не корректный номер вопроса. Введите число от 1 до {_questionsCount}", $"Введен не верный номер вопроса {questionNumber}");
                return;
            }
            _currentQuestionIndex = index;   
        }
    }

    private bool ValidateModel(QuestionnaireModel model)
    {
        if (string.IsNullOrEmpty(model.Fio))
        {
            MessageAndLog("Не введено ФИО");
            _currentQuestionIndex = 0;
            return false;
        }

        if (model.BirthDate == DateTime.MinValue)
        {
            MessageAndLog("Не введена дату рождения");
            _currentQuestionIndex = 1;
            return false;
        }

        if (string.IsNullOrEmpty(model.Language))
        {
            MessageAndLog("Не введен язык программирования");
            _currentQuestionIndex = 2;
            return false;
        }

        if (string.IsNullOrEmpty(model.Phone))
        {
            MessageAndLog("Не введен номер телефона");
            _currentQuestionIndex = 4;
            return false;
        }

        return true;
    }
    private void GoToPrevQuestion(string[] args)
    {
        if (_currentQuestionIndex > 0)
        {
            _currentQuestionIndex--;
            return;
        }

        MessageAndLog("Вы уже на первом вопросе", $"Попытка перейти к вопросу с индексом {_currentQuestionIndex-1}");
    }

    private bool MapAnswers(string? propertyName, string? answer)
    {
        answer = answer.Trim();
        
        switch (propertyName)
        {
            case nameof(_model.Fio):
            {
                _model.Fio = answer;
                return true;
            }
            case nameof(_model.BirthDate):
            {
                if (DateTime.TryParseExact(answer, "dd.MM.yyyy", null, DateTimeStyles.None, out var date))
                {
                    _model.BirthDate = date;
                    return true;
                }
                MessageAndLog("Не верный формат даты, верный формат ДД.ММ.ГГГГ",
                    $"Не верный формат даты answer = '{answer}'");
                return false;
            }
            case nameof(_model.Experience):
            {
                if (int.TryParse(answer, out var intValue))
                {
                    _model.Experience = intValue;
                    return true;
                }
                MessageAndLog("Не верный формат числа, принимаются только целые числа",
                    $"Не верный формат числа answer = '{answer}'");
                return false;
            }
            case nameof(_model.Language):
            {
                var cleanAnswer = answer.ToUpper();
                var language =
                    ProgrammingLanguageHelper.SupportedLanguages.FirstOrDefault(_ => _.ToUpper() == cleanAnswer);
                if (!string.IsNullOrEmpty(language))
                {
                    _model.Language = language;
                    return true;
                }
                MessageAndLog($"Не поддерживаемый язык программирования, введите один из данных языков: {string.Join(", ", ProgrammingLanguageHelper.SupportedLanguages)}", 
                    $"Не поддерживаемый язык программирования answer = '{answer}'");
                return false;
            }
            case nameof(_model.Phone):
            {
                var pattern = @"^(\+)?((\d{2,3}) ?\d|\d)(([ -]?\d)|( ?(\d{2,3}) ?)){5,12}\d$";
                if (Regex.IsMatch(answer, pattern))
                {
                    _model.Phone = answer;
                    return true;
                }
                MessageAndLog($"Не верный формат мобильного телефона.",
                    $"Не верный формат мобильного телефона answer = '{answer}'");
                return false;
            }
        }
        MessageAndLog("Не известное значение", 
            $"Не известное значение свойства propertyName = {propertyName}, answer = '{answer}'");
        return false;
    }

    private string GetPropertyNameFromQuestionIndex(int questionIndex)
    {
        switch (questionIndex)
        {
            case 0 : return nameof(_model.Fio);
            case 1 : return nameof(_model.BirthDate);
            case 2 : return nameof(_model.Language);
            case 3 : return nameof(_model.Experience);
            case 4 : return nameof(_model.Phone);
        }
        return String.Empty;
    }

    private void MessageAndLog(string message, string? logMessage=null)
    {
        logMessage ??= message;
        _logger.LogWarning(logMessage);

        Console.WriteLine(message);
    }
}