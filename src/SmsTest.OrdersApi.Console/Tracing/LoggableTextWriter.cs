using System.Text;

using Microsoft.Extensions.Logging;

namespace SmsTest.OrdersApi.Console.Tracing;

public sealed class LoggableConsoleWriter : TextWriter
{
    private readonly ILogger _logger;

    private readonly TextWriter _textWriter;

    public LoggableConsoleWriter(TextWriter textWriter, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(textWriter);
        this._textWriter = textWriter;

        ArgumentNullException.ThrowIfNull(logger);
        this._logger = logger;
    }

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(string? value)
    {
        this._textWriter.Write(value);

        if (!string.IsNullOrEmpty(value))
        {
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            this._logger.LogInformation(value);
        }
    }

    public override void WriteLine(string? value)
    {
        this._textWriter.WriteLine(value);

        if (!string.IsNullOrEmpty(value))
        {
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            this._logger.LogInformation(value);
        }
    }
}
