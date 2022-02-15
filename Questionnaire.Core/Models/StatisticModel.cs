namespace Questionnaire.Core.Models;

public class StatisticModel
{
    /// <summary>
    /// Средний возраст
    /// </summary>
    public int AvgAge { get; set; }
    /// <summary>
    /// Самый популярный язык
    /// </summary>
    public string TopLanguage { get; set; }
    /// <summary>
    /// Самый опытный
    /// </summary>
    public string ExperiencedProgrammer { get; set; }
}