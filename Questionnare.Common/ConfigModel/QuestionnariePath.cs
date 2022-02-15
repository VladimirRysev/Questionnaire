namespace Questionnaire.Common.ConfigModel;

public class QuestionnariePath
{
    /// <summary>
    /// Путь к папке хранения анкет
    /// </summary>
    public string Path { get; set; }
    /// <summary>
    /// Путь к шаблону анкеты
    /// </summary>
    public string TemplatePath { get; set; }
}