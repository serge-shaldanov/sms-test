namespace SmsTest.VariablesApp.Models.Services;

public interface IVariableCommentStorageService
{
    string? GetComment(string variableName);

    void SetComment(string variableName, string? commentText);
}
