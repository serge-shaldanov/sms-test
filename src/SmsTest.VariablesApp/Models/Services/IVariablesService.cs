using SmsTest.VariablesApp.Models.Entities;

namespace SmsTest.VariablesApp.Models.Services;

public interface IVariablesService
{
    IEnumerable<Variable> GetVariables();

    void UpdateVariable(Variable variable);
}
