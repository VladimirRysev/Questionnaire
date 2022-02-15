using System.Reflection;
using Microsoft.Extensions.Logging;
using Questionnaire.Constants;

namespace Questionnaire.Commands;

public class CommandsManager : ICommandsManager
{
    private Dictionary<string, ICommand> _commandsList;
    private IEnumerable<ICommand> _allCommands;
    private ILogger<CommandsManager> _logger;
    public CommandsManager(IEnumerable<ICommand> allCommands, ILogger<CommandsManager> logger)
    {
        _allCommands = allCommands;
        _logger = logger;
        _commandsList = new Dictionary<string, ICommand>();
        InitCommands();
    }

    private void InitCommands()
    {
        foreach (var command in _allCommands)
        {
            if (string.IsNullOrEmpty(command.CommandName))
            {
                var type = command.GetType();
                _logger.LogWarning($"Обнаружена команда без имени {type.Name}");
                Console.WriteLine($"Обнаружена команда без имени {type.Name}");
                continue;
            }
            _commandsList.Add(command.CommandName, command);
        }
    }

    public void Execute(string command, string[] args)
    {
        if (string.IsNullOrEmpty(command))
        {
            return;
        }
        if (command.Equals(CommandsConstants.HelpCommandName))
        {
            Help();
            return;
        }

        if (!_commandsList.ContainsKey(command))
        {
            Console.WriteLine("Команда не распознана");
            return;
        }
        var currentCommand = _commandsList[command];
        currentCommand.Execute(args);
    }

    private void Help()
    {
        foreach (var command in _commandsList)
        {
            command.Value.Help();
        }
        Console.WriteLine($"Для выхода введите {CommandsConstants.ExitCommand}");
    }
}