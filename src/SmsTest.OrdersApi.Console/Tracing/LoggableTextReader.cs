using Microsoft.Extensions.Logging;

namespace SmsTest.OrdersApi.Console.Tracing;

public sealed class LoggableTextReader : TextReader
{
    private readonly ILogger _logger;

    private readonly TextReader _textReader;

    public LoggableTextReader(TextReader textReader, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(textReader);
        this._textReader = textReader;

        ArgumentNullException.ThrowIfNull(logger);
        this._logger = logger;
    }

    public override string? ReadLine()
    {
        string? line = this._textReader.ReadLine();

        this._logger.LogInformation(message: "User input: \"{Line}\"", line ?? "<empty>");

        return line;
    }
}
