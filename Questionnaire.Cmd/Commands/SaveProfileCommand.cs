using Microsoft.Extensions.Logging;
using Questionnaire.Core.Services.QuestionnaireFiles;
using Questionnaire.Services;
using Questionnaire.Services.Questionnaire;
using Questionnaire.Services.Temp;

namespace Questionnaire.Commands;

public class SaveProfileCommand : ICommand
{
    public string CommandName { get; }
    public string Description { get; }
    private IQuestionnaireFileService _questionnaireFileService;
    private ITempQuestionnaireService _tempQuestionnaireService;
    private ILogger<SaveProfileCommand> _logger;

    public SaveProfileCommand(ITempQuestionnaireService tempQuestionnairesService, IQuestionnaireFileService questionnaireFileService, ILogger<SaveProfileCommand> logger)
    {
        _tempQuestionnaireService = tempQuestionnairesService;
        _questionnaireFileService = questionnaireFileService;
        _logger = logger;
        CommandName = "-save";
        Description = "Сохраняет заполненную анкету";
    }

    public void Help()
    {
        Console.WriteLine($"{CommandName}: {Description}");
    }

    public void Execute(string[] args)
    {
        var model = _tempQuestionnaireService.GetTempQuestionnaire();
        var result = _questionnaireFileService.Save(model);
        Console.WriteLine(result.Message);
    }
}