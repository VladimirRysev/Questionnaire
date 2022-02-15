// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Questionnaire.Commands;
using Questionnaire.Common.ConfigModel;
using Questionnaire.Core.Services.QuestionnaireFiles;
using Questionnaire.Core.Services.Questions;
using Questionnaire.Core.Services.Statistic;
using Questionnaire.Services.Questionnaire;
using Questionnaire.Services.Temp;

var builder = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
    .AddJsonFile("appsettings.json", false, true);

IConfiguration config = builder.Build();

var questionnariePath = new QuestionnariePath();
config.Bind("QuestionnairePath", questionnariePath);

var services = new ServiceCollection()
    .AddSingleton(config)
    .AddLogging(l =>
    {
        l.ClearProviders();
        l.SetMinimumLevel(LogLevel.Trace);
        l.AddNLog("nlog.config");
    });
services.AddSingleton(questionnariePath);
services.AddSingleton<IQuestionnairesService, QuestionnairesService>();
services.AddSingleton<ICommandsManager, CommandsManager>();
services.AddSingleton<IQuestionsService, QuestionsService>();
services.AddSingleton<IQuestionnaireFileService, QuestionnaireFileService>();
services.AddSingleton<IQuestionnairesService, QuestionnairesService>();
services.AddSingleton<IStatisticService, StatisticService>();
services.AddSingleton<ICommand, NewProfileCommand>();
services.AddSingleton<ICommand, SaveProfileCommand>();
services.AddSingleton<ICommand, ListCommand>();
services.AddSingleton<ICommand, StatisticCommand>();
services.AddSingleton<ICommand, FindProfileCommand>();
services.AddSingleton<ICommand, DeleteProfileCommand>();
services.AddSingleton<ICommand, ListToDayCommand>();
services.AddSingleton<ICommand, ZipCommand>();
services.AddSingleton<ITempQuestionnaireService, TempQuestionnaireService>();

var serv = services.BuildServiceProvider();
var app = serv.GetService<IQuestionnairesService>();
app.Run();