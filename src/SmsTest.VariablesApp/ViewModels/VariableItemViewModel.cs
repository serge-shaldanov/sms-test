using CommunityToolkit.Mvvm.ComponentModel;

using SmsTest.VariablesApp.Models.Entities;
using SmsTest.VariablesApp.ViewModels.Events;

namespace SmsTest.VariablesApp.ViewModels;

public partial class VariableItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _comment;

    [ObservableProperty]
    private string _value;

    public VariableItemViewModel(Variable model)
    {
        ArgumentNullException.ThrowIfNull(model);
        this.Name     = model.Name;
        this._value   = model.Value;
        this._comment = model.Comment ?? string.Empty;
    }

    public string Name { get; }

    public event EventHandler<VariableChangedEventArgs>? VariableChanged;

    partial void OnValueChanged(string value)
    {
        this.RaiseUpdateVariable();
    }

    partial void OnCommentChanged(string value)
    {
        this.RaiseUpdateVariable();
    }

    private void RaiseUpdateVariable()
    {
        string  value   = this.Value.Trim();
        string? comment = !string.IsNullOrWhiteSpace(this.Comment) ? this.Comment.Trim() : null;

        Variable variable = new Variable(this.Name, value, comment);

        this.VariableChanged?.Invoke(sender: this, e: new VariableChangedEventArgs(variable));
    }
}
