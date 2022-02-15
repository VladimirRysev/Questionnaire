namespace Questionnaire.Commands;

public interface ICommandsManager
{
    void Execute(string command, string[] args);
}