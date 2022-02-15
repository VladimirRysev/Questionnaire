using Microsoft.Extensions.Logging;
using Questionnaire.Core.Services.QuestionnaireFiles;

namespace Questionnaire.Commands;

public class FindProfileCommand : ICommand
{
    public string CommandName { get; }
    public string Description { get; }
    private IQuestionnaireFileService _questionnaireFileService;
    private ILogger<FindProfileCommand> _logger;

    public FindProfileCommand(IQuestionnaireFileService questionnaireFileService, ILogger<FindProfileCommand> logger)
    {
        _questionnaireFileService = questionnaireFileService;
        _logger = logger;
        CommandName = "-find";
        Description = "Находит анкету по указанному имени файла и показать данные анкеты в консоль. -find <Имя файла анкеты>";
    }

    public void Help()
    {
        Console.WriteLine($"{CommandName}: {Description}");
    }

    public void Execute(string[] args)
    {
        if (args.Length == 0  || string.IsNullOrEmpty(args[0]))
        {
            _logger.LogWarning("Не введено имя файла");
            Console.WriteLine("Не введено имя файла");
        }
        var name = string.Join(" ", args.Select(_=>_.Trim().Normalize()));
        var result = _questionnaireFileService.GetByName(name);
        if (!result.Success)
        {
            Console.WriteLine(result.Message);
            return;
        }

        var model = result.Result;
        Console.WriteLine($"ФИО: {model.Fio}");
        Console.WriteLine($"Дата рождения: {model.BirthDate:dd.MM.yyy}");
        Console.WriteLine($"Любимый язык программирования: {model.Language}");
        Console.WriteLine($"Опыт программирования на указанном языке: {model.Experience}");
        Console.WriteLine($"Мобильный телефон: {model.Phone}");
    }
}