using Microsoft.Extensions.Options;
using Microsoft.Isam.Esent.Collections.Generic;

namespace SmsTest.VariablesApp.Models.Services;

/// <summary>
/// Используем готовую реализацию персистентного словаря на базе движка ESENT:
/// https://github.com/microsoft/ManagedEsent/blob/master/Documentation/PersistentDictionaryDocumentation.md
/// Развитие:
/// При появлении большого количества персистентных данных разного характера, при появлении связей между этими данными,
/// имеет смысл перейти на более универсальное хранилище по типу SQLite.
/// </summary>
public sealed class PersistentVariableCommentStorageService : IVariableCommentStorageService, IDisposable
{
    private readonly PersistentDictionary<string, string> _comments;

    private bool _isDisposed;

    public PersistentVariableCommentStorageService(
        IOptions<VariableOptions> options
    )
    {
        this._comments   = new PersistentDictionary<string, string>(options.Value.CommentsPath);
        this._isDisposed = false;
    }

    public void Dispose()
    {
        if (this._isDisposed)
        {
            return;
        }

        this._comments.Dispose();

        this._isDisposed = true;
    }

    public string? GetComment(string variableName)
    {
        ObjectDisposedException.ThrowIf(this._isDisposed, instance: this);
        ArgumentException.ThrowIfNullOrEmpty(variableName);

        return this._comments.TryGetValue(variableName, value: out string variableValue)
            ? variableValue
            : null;
    }

    public void SetComment(string variableName, string? commentText)
    {
        ObjectDisposedException.ThrowIf(this._isDisposed, instance: this);
        ArgumentException.ThrowIfNullOrEmpty(variableName);

        if (string.IsNullOrEmpty(commentText))
        {
            this._comments.Remove(variableName);
        }
        else
        {
            this._comments[variableName] = commentText;
        }

        this._comments.Flush();
    }
}
