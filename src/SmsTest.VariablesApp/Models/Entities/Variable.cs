namespace SmsTest.VariablesApp.Models.Entities;

public sealed class Variable
{
    public Variable(string name, string value, string? comment = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        this.Name = name;

        ArgumentNullException.ThrowIfNull(value);
        this.Value = value;

        this.Comment = comment;
    }

    public string Name { get; }

    public string Value { get; }

    public string? Comment { get; }

    public override string ToString()
    {
        return $"[{this.Name} = \"{this.Value}\" (комментарий: \"{this.Comment}\")]";
    }
}
