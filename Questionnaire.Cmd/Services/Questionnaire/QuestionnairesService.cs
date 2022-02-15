using Microsoft.Extensions.Logging;
using Questionnaire.Commands;
using Questionnaire.Constants;
using Questionnaire.Core.Models;

namespace Questionnaire.Services.Questionnaire;

public class QuestionnairesService : IQuestionnairesService
{
    private ILogger<QuestionnairesService> _logger;
    private ICommandsManager _commandsManager;

    public QuestionnairesService(ILogger<QuestionnairesService> logger, ICommandsManager commandsManager)
    {
        _logger = logger;
        _commandsManager = commandsManager;
    }

    public void Run()
    {
        _logger.LogInformation("Run QuestionnairesService");
        
        var helloStr = $"Выберите действие:";
        while (true)
        {
            Console.WriteLine(helloStr);
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                continue;
            }
            
            var inputList = input.Split(" ").ToList();
            var command = inputList[0];

            if (command.Equals(CommandsConstants.ExitCommand))
            {
                return;
            }
            var args = inputList.Count > 1 ? inputList.Skip(1).ToArray():null;
            _commandsManager.Execute(command, args);
            Console.WriteLine();
        }
    }
}