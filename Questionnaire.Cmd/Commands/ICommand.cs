namespace Questionnaire.Commands;

public interface ICommand
{   
    public string CommandName { get; }
    public string Description { get; }
    public void Help();
    void Execute(string[] args);
}