using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SmsTest.VariablesApp.Models.Entities;

namespace SmsTest.VariablesApp.Models.Services;

public sealed class EnvironmentVariablesService : IVariablesService
{
    private readonly IVariableCommentStorageService _comments;

    private readonly ILogger<EnvironmentVariablesService> _logger;

    private readonly IEnumerable<string> _variableNames;

    public EnvironmentVariablesService(
        IVariableCommentStorageService       commentStorageService,
        IOptions<VariableOptions>            options,
        ILogger<EnvironmentVariablesService> logger
    )
    {
        ArgumentNullException.ThrowIfNull(commentStorageService);
        this._comments = commentStorageService;

        ArgumentNullException.ThrowIfNull(options);
        this._variableNames = options.Value.Names;

        ArgumentNullException.ThrowIfNull(logger);
        this._logger = logger;
    }

    public IEnumerable<Variable> GetVariables()
    {
        foreach (string variableName in this._variableNames)
        {
            string variableValue =
                Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User) ?? string.Empty;

            string? variableComment = this._comments.GetComment(variableName);

            Variable variable = new Variable(variableName, variableValue, variableComment);

            this._logger.LogInformation(
                message: "Считано очередное значение переменной: {Variable}",
                variable
            );

            yield return variable;
        }
    }

    public void UpdateVariable(Variable variable)
    {
        ArgumentNullException.ThrowIfNull(variable);

        string? oldVariableValue = Environment.GetEnvironmentVariable(variable.Name, EnvironmentVariableTarget.User);

        // В ТЗ нет указаний касательно того, какого типа должна быть переменная, поэтому будем использовать
        // общедоступную переменную уровня пользователя.
        // Также, в ТЗ упомянуто, что приложение не должно налагать фактических ограничений на длину значений у
        // переменных, тем не менее, у самой хост-системы могут быть свои ограничения, с которыми мы ничего не можем 
        // сделать.
        // Поэтому здесь считаем, что ограничения самой ОС находятся вне области задачи.
        Environment.SetEnvironmentVariable(variable.Name, variable.Value, EnvironmentVariableTarget.User);

        try
        {
            // В ТЗ нет указаний, что делать с данными из столбца "Комментарий".
            // Переменные среды не предоставляют встроенных средств для хранения подобных данных.
            // В appsettings.json такие данные хранить нельзя, поскольку это файл персистентной конфигурации.
            // Поэтому в качестве источника хранения будем использовать вспомогательный сервис с записью в
            // файл из локального каталога текущего пользователя (к которому всегда есть доступ на запись).
            this._comments.SetComment(variable.Name, variable.Comment);
        }
        catch
        {
            // Откатываем изменения в системе, если не получилось завершить операцию.
            Environment.SetEnvironmentVariable(variable.Name, oldVariableValue, EnvironmentVariableTarget.User);

            throw;
        }

        this._logger.LogInformation(
            message: "Записано новое значение переменной: {Variable} (старое значение: \"{OldVariableValue}\")",
            variable,
            oldVariableValue
        );
    }
}
