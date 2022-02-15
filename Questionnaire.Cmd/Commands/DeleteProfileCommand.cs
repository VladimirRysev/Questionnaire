using Microsoft.Extensions.Logging;
using Questionnaire.Core.Services.QuestionnaireFiles;

namespace Questionnaire.Commands;

public class DeleteProfileCommand : ICommand
{
    public string CommandName { get; }
    public string Description { get; }
    private IQuestionnaireFileService _questionnaireFileService;
    private ILogger<DeleteProfileCommand> _logger;
    public DeleteProfileCommand(IQuestionnaireFileService questionnaireFileService, ILogger<DeleteProfileCommand> logger)
    {
        _questionnaireFileService = questionnaireFileService;
        _logger = logger;
        CommandName = "-delete";
        Description = "Удаляет указанную анкету. -delete <Имя файла анкеты>";
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

        var result = _questionnaireFileService.DeleteByName(name);
        Console.WriteLine(result.Message);
    }
}