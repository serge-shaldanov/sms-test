using SmsTest.VariablesApp.Models.Entities;

namespace SmsTest.VariablesApp.ViewModels.Events;

public sealed class VariableChangedEventArgs : EventArgs
{
    public VariableChangedEventArgs(Variable newVariable)
    {
        ArgumentNullException.ThrowIfNull(newVariable);
        this.Variable = newVariable;
    }

    public Variable Variable { get; }
}
