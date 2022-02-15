using Questionnaire.Common.Filters;
using Questionnaire.Core.Services.QuestionnaireFiles;

namespace Questionnaire.Commands;

public class ListCommand : ICommand
{
    public string CommandName { get; }
    public string Description { get; }

    private IQuestionnaireFileService _questionnaireFileService;
    
    public ListCommand(IQuestionnaireFileService questionnaireFileService)
    {
        _questionnaireFileService = questionnaireFileService;
        CommandName = "-list";
        Description = "Отображает список названий файлов всех сохраненных анкет";
    }

    public void Help()
    {
        Console.WriteLine($"{CommandName}: {Description}");
    }

    public void Execute(string[] args)
    {
        var result = _questionnaireFileService.GetFileList(new QuestionnairesFileFilter());
        if (!result.Success)
        {
            Console.WriteLine(result.Message);
            return;
        }

        var list = result.Result;
        if (list?.Any() == true)
        {
            Console.WriteLine("Список сохраненных анкет:");
            foreach (var file in list)
            {
                Console.WriteLine(file);
            }
        }
        else
        {
            Console.WriteLine("Пока нет сохраненных анкет");
        }
    }
}