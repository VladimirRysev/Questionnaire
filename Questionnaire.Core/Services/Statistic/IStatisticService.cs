using Questionnaire.Core.Models;

namespace Questionnaire.Core.Services.Statistic;

public interface IStatisticService
{
    ServiceResult<StatisticModel> GetStatistic();
}