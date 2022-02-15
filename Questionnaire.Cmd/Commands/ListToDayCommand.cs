using Questionnaire.Common.Filters;
using Questionnaire.Core.Services.QuestionnaireFiles;

namespace Questionnaire.Commands;

public class ListToDayCommand : ICommand
{
    public string CommandName { get; }
    public string Description { get; }

    private IQuestionnaireFileService _questionnaireFileService;


    public ListToDayCommand(IQuestionnaireFileService questionnaireFileService)
    {
        _questionnaireFileService = questionnaireFileService;
        CommandName = "-list_today";
        Description = "Отображает список названий файлов всех сохраненных анкет, созданных сегодня";
    }
    public void Help()
    {
        Console.WriteLine($"{CommandName}: {Description}");
    }

    public void Execute(string[] args)
    {
       
        var result = _questionnaireFileService.GetFileList(new QuestionnairesFileFilter()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
        });
        if (!result.Success)
        {
            Console.WriteLine(result.Message);
            return;
        }

        var list = result.Result;
        if (list?.Any() == true)
        {
            Console.WriteLine("Список сохраненных сегодня анкет:");
            foreach (var file in list)
            {
                Console.WriteLine(file);
            }
        }
        else
        {
            Console.WriteLine("Пока нет сохраненных сегодня анкет");
        }
    }
}