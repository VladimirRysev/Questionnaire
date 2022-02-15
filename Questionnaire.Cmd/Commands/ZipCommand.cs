using Microsoft.Extensions.Logging;
using Questionnaire.Core.Services.QuestionnaireFiles;

namespace Questionnaire.Commands;

public class ZipCommand : ICommand
{
    public string CommandName { get; }
    public string Description { get; }
    private IQuestionnaireFileService _questionnaireFileService;
    private ILogger<ZipCommand> _logger;

    public ZipCommand(IQuestionnaireFileService questionnaireFileService, ILogger<ZipCommand> logger)
    {
        _questionnaireFileService = questionnaireFileService;
        _logger = logger;
        CommandName = "-zip";
        Description = "Запаковывает указанную анкету в архив и сохраняет архив по указанному пути. -zip -i <Имя файла анкеты> -o <Путь для сохранения архива>";
    }

    public void Help()
    {
        Console.WriteLine($"{CommandName}: {Description}");
    }

    public void Execute(string[] args)
    {
        if (args.Length < 4)
        {
            var argsStr = string.Join(" ", args);
            ShowError(argsStr);
        }

        var fileName = string.Empty;
        var archivePath = string.Empty;
        var flag = 0;
        
        foreach (var s in args)
        {
            if (s.Equals("-i"))
            {
                continue;
            }

            if (s.Equals("-o"))
            {
                flag = 1;
                continue;
            }

            switch (flag)
            {
                case 0: fileName += string.IsNullOrEmpty(fileName)? s : $" {s}"; break;
                case 1: archivePath += string.IsNullOrEmpty(archivePath)? s : $" {s}"; break;
            }
        }

        if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(archivePath))
        {
            var argsStr = string.Join(" ", args);
            ShowError(argsStr);
        }

        var result = _questionnaireFileService.CreateArchive(fileName, archivePath);
        Console.WriteLine(result.Message);
    }

    private void ShowError(string argsStr)
    {
        _logger.LogWarning($"Не верный формат ввода args = {argsStr}");
        Console.WriteLine("Не верный формат ввода");
        Help();
    }
}