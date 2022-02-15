using Microsoft.Extensions.Logging;
using Questionnaire.Common.Filters;
using Questionnaire.Core.Models;
using Questionnaire.Core.Services.QuestionnaireFiles;

namespace Questionnaire.Core.Services.Statistic;

public class StatisticService : IStatisticService
{
    private readonly ILogger<StatisticService> _logger;
    private readonly IQuestionnaireFileService _questionnaireFileService;

    public StatisticService(IQuestionnaireFileService questionnaireFileService, ILogger<StatisticService> logger)
    {
        _questionnaireFileService = questionnaireFileService;
        _logger = logger;
    }

    public ServiceResult<StatisticModel> GetStatistic()
    {
        try
        {
            var result = _questionnaireFileService.GetList(new QuestionnairesFileFilter());
            if (!result.Success) return ServiceResult<StatisticModel>.ErrorResult(result.Message);

            var list = result.Result;
            var model = new StatisticModel();
            var now = DateTime.Today;
            model.AvgAge = list?.Any() != true ? 0 : (int)list.Select(_ => ((now - _.BirthDate).Days / 365)).Average();
            model.ExperiencedProgrammer = list?.OrderByDescending(_ => _.Experience).FirstOrDefault()?.Fio??"";
            model.TopLanguage = list?.GroupBy(_ => _.Language, (language, items) => new
            {
                Count = items.Count(),
                Lanmguage = language
            }).OrderByDescending(_ => _.Count).FirstOrDefault()?.Lanmguage??"";
            return ServiceResult<StatisticModel>.SuccessResult(model);
        }
        catch (Exception e)
        {
            _logger.LogError("При получении статистики возникла ошибка.\n {0}", e);
            return ServiceResult<StatisticModel>.ErrorResult("При получении статистики возникла ошибка");
        }
    }
}