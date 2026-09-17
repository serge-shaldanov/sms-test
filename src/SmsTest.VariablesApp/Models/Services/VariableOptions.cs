namespace SmsTest.VariablesApp.Models.Services;

public sealed class VariableOptions
{
    public const string SectionName = "Variables";

    public IEnumerable<string> Names { get; set; } = [];

    public string CommentsPath { get; set; } = string.Empty;
}
