using Questionnaire.Common.Extensions;
using Questionnaire.Core.Services.Statistic;

namespace Questionnaire.Commands;

public class StatisticCommand : ICommand
{
    private readonly IStatisticService _statisticService;

    public StatisticCommand(IStatisticService statisticService)
    {
        _statisticService = statisticService;
        CommandName = "-statistics";
        Description = "Показать статистику всех заполненных анкет";
    }

    public string CommandName { get; }
    public string Description { get; }

    public void Help()
    {
        Console.WriteLine($"{CommandName}: {Description}");
    }

    public void Execute(string[] args)
    {
        var result = _statisticService.GetStatistic();
        if (result.Success)
        {
            var model = result.Result;
            Console.WriteLine($"Средний возраст всех опрошенных: {model.AvgAge.GetYears()}");
            Console.WriteLine($"Самый популярный язык программирования: {model.TopLanguage}");
            Console.WriteLine($"Самый опытный программист: {model.ExperiencedProgrammer}");
        }
        else
        {
            Console.WriteLine(result.Message);
        }
    }
}