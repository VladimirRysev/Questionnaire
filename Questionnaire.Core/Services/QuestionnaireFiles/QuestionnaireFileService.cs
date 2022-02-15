using System.Globalization;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Questionnaire.Common.ConfigModel;
using Questionnaire.Common.Filters;
using Questionnaire.Core.Models;

namespace Questionnaire.Core.Services.QuestionnaireFiles;

public class QuestionnaireFileService : IQuestionnaireFileService
{
    private readonly ILogger<QuestionnaireFileService> _logger;
    private string _path;
    private string _templatePath;
    public QuestionnaireFileService(QuestionnariePath questionnairePath, ILogger<QuestionnaireFileService> logger)
    {
        _path = Path.Combine(Environment.CurrentDirectory, questionnairePath.Path);
        _templatePath = Path.Combine(Environment.CurrentDirectory, questionnairePath.TemplatePath);
        _logger = logger;
    }

    public ServiceResult<QuestionnaireModel?> GetByName(string name)
    {
        name = name.EndsWith(".txt") ? name : name + ".txt";
        var path = Path.Combine(_path, name);
        return ReadAndParse(path);
    }

    public ServiceResult Save(QuestionnaireModel questionnaire)
    {
        if (questionnaire == null)
        {
            _logger.LogWarning("Попытка сохранить пустую анкету");
            return ServiceResult.ErrorResult("Анкета еще не заполнена");
        }
        var path = Path.Combine(_path, $"{questionnaire.Fio}.txt");
        return ParseAndSave(path, questionnaire);
    }

    public ServiceResult<List<string>> GetFileList(QuestionnairesFileFilter filter)
    {
        try
        {
            var list = new List<string>();
            if (Directory.Exists(_path))
            {
                var dirInfo = new DirectoryInfo(_path);
                var files = dirInfo.GetFiles();
                if (filter.StartDate.HasValue)
                {
                    files = files.Where(_ => _.CreationTime >= filter.StartDate).ToArray();
                }
                if (filter.EndDate.HasValue)
                {
                    files = files.Where(_ => _.CreationTime >= filter.StartDate).ToArray();
                }

                list = files.Select(_ => _.Name).ToList();
            }
            return ServiceResult<List<string>>.SuccessResult(list);
        }
        catch (Exception e)
        {
            _logger.LogError("При получении списка файлов произошла ошибка.\n {0}", e);
            return ServiceResult<List<string>>.ErrorResult("При получении списка файлов произошла ошибка");
        }
    }
    
    public ServiceResult<List<QuestionnaireModel>> GetList(QuestionnairesFileFilter filter)
    {
        var result = new List<QuestionnaireModel>();
        try
        {
            var filesResult = GetFileList(filter);
            if (!filesResult.Success)
            {
                return ServiceResult<List<QuestionnaireModel>>.ErrorResult(filesResult.Message);
            }

            var files = filesResult.Result;
            foreach (var file in files)
            {
                var path = Path.Combine(_path, file);
                var modelResult = ReadAndParse(path);
                if (modelResult.Success)
                {
                    result.Add(modelResult.Result);
                }
            }

            if (filter.StartDate.HasValue) result = result.Where(_ => _.Date >= filter.StartDate.Value).ToList();
            if (filter.EndDate.HasValue) result = result.Where(_ => _.Date <= filter.EndDate.Value).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError("При получении списка анкет произошла ошибка.\n {0}", e);
            return ServiceResult<List<QuestionnaireModel>>.ErrorResult("При получении списка анкет произошла ошибка");
        }
        return ServiceResult<List<QuestionnaireModel>>.SuccessResult(result);
    }

    public ServiceResult CreateArchive(string fileName, string path)
    {
        if (!fileName.ToLower().EndsWith(".txt")) fileName += ".txt";
        var filePath = Path.Combine(_path, fileName);
        if (!File.Exists(filePath))
        {
            _logger.LogWarning($"Анкета не найдена filePath = {filePath}");
            return ServiceResult.ErrorResult("Анкета не найдена");
        }

        try
        {

            using (FileStream sourceStream = new FileStream(filePath, FileMode.Open))
            {
                using (FileStream targetStream = File.Create(path))
                {
                    using (var zip = new ZipArchive(targetStream, ZipArchiveMode.Create))
                    {
                        ZipArchiveEntry readmeEntry = zip.CreateEntry(fileName);
                        using (var stream = readmeEntry.Open())
                        {
                            sourceStream.CopyTo(stream);
                        }
                    }
                }
            }
        }
        catch (DirectoryNotFoundException e)
        {
            _logger.LogError("Ошибка при архивации файла.\n {0}", e);
            return ServiceResult.ErrorResult("Не верный путь сохранения файла");
        }
        catch (Exception e)
        {
            _logger.LogError("Ошибка при архивации файла.\n {0}", e);
            return ServiceResult.ErrorResult("Произошла ошибка при архивации файла");
        }
        return ServiceResult.SuccessResult("Архивация прошла успешно");
    }

    public ServiceResult DeleteByName(string name)
    {
        if (!name.ToLower().EndsWith(".txt")) name += ".txt";
        var path = Path.Combine(_path, name);

        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception e)
        {
            _logger.LogError("При удалении файла произошла ошибка.\n {0}", e);
            ServiceResult.ErrorResult("При удалении файла произошла ошибка");
        }
        return ServiceResult.SuccessResult("Удаление прошло успешно");
    }

    private ServiceResult ParseAndSave(string path, QuestionnaireModel? questionnaire)
    {
        try
        {
            var lines = new List<string>();
            if (!File.Exists(_templatePath))
            {
                _logger.LogWarning($"Файл шаблона не найден по пути {_templatePath}");
                return ServiceResult.ErrorResult("Файл шаблона не найден");
            }
            var template = File.ReadAllText(_templatePath);
            if (string.IsNullOrEmpty(template))
            {
                _logger.LogWarning("Файл шаблона пустой");
                return ServiceResult.ErrorResult("Файл шаблона пустой");
            }

            if (questionnaire != null)
            {
                var type = questionnaire.GetType();
                foreach (var property in type.GetProperties())
                {
                    var patern = @"\<" + property.Name + @"\>";
                    var valueStr = string.Empty;
                    var value = property.GetValue(questionnaire);
                    switch (property.PropertyType.Name)
                    {
                        case "DateTime":
                            {
                                valueStr = (value as DateTime?)?.ToString("dd.MM.yyyy");
                            }
                            break;
                        default:
                            valueStr = value.ToString();
                            break;
                    }

                    template = Regex.Replace(template, patern, valueStr);
                }
            }
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            File.WriteAllText(path, template);
        }
        catch (Exception e)
        {
            _logger.LogError("При сохранении анкеты произошла ошибка.\n {0}", e);
            ServiceResult.ErrorResult("При сохранении анкеты произошла ошибка");
        }
        return ServiceResult.SuccessResult("Сохранение прошло успешно");
    }

    private ServiceResult<QuestionnaireModel?> ReadAndParse(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                _logger.LogWarning($"Файл анкеты не найден path = {path}");
                return ServiceResult<QuestionnaireModel?>.ErrorResult("Файл анкеты не найден");
            }

            var templateLines = File.ReadAllLines(_templatePath);
            if (!templateLines.Any())
            {
                _logger.LogWarning($"Файл шаблона не найден по пути {_templatePath}");
                return ServiceResult<QuestionnaireModel?>.ErrorResult("Файл шаблона не найден");
            }
            var lines = File.ReadAllLines(path);
            if (!lines.Any())
            {
                _logger.LogWarning($"Файл анкеты пустой path = {path}");
                return ServiceResult<QuestionnaireModel?>.ErrorResult("Файл анкеты пустой");
            }
            var questionnare = new QuestionnaireModel();
            var type = questionnare.GetType();
            var patern = @"\<(.+?)\>";

            for (var i = 0; i < templateLines.Length; i++)
            {
                var templateLine = templateLines[i];
                var propName = Regex.Match(templateLine, patern)?.Value ?? string.Empty;
                propName = propName.Replace("<", "").Replace(">", "");
                var prop = type.GetProperty(propName);
                if (prop != null)
                {
                    var valueStr = lines[i].Substring(lines[i].IndexOf(':') + 1).Trim();
                    object? value = default;

                    switch (prop.PropertyType.Name)
                    {
                        case "DateTime":
                            {
                                if (DateTime.TryParseExact(valueStr, "dd.MM.yyyy", null, DateTimeStyles.None, out var date))
                                {
                                    value = date;
                                }
                            }
                            break;
                        case "String":
                            value = valueStr;
                            break;
                        case "Int32":
                            {
                                int intValue;
                                int.TryParse(valueStr, out intValue);
                                value = intValue;
                            }
                            break;
                    }

                    prop.SetValue(questionnare, value);
                }
            }

            return ServiceResult<QuestionnaireModel?>.SuccessResult(questionnare); ;
        }
        catch (Exception e)
        {
            _logger.LogError("При парсинге файла анкеты произошла ошибка.\n {0}", e);
            return ServiceResult<QuestionnaireModel?>.ErrorResult("При парсинге файла анкеты произошла ошибка");
        }
    }
}