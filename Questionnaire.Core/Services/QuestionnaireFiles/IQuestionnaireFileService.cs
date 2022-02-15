using Questionnaire.Common.Filters;
using Questionnaire.Core.Models;

namespace Questionnaire.Core.Services.QuestionnaireFiles;

public interface IQuestionnaireFileService
{
    ServiceResult<QuestionnaireModel?> GetByName(string name);
    ServiceResult Save(QuestionnaireModel questionnaire);
    ServiceResult<List<string>> GetFileList(QuestionnairesFileFilter filter);
    ServiceResult<List<QuestionnaireModel>> GetList(QuestionnairesFileFilter filter);
    ServiceResult DeleteByName(string name);
    ServiceResult CreateArchive(string fileName, string path);
}