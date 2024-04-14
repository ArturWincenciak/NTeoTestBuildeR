using JetBrains.Annotations;

namespace NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;

[PublicAPI]
public class AppArgumentException(string message)
    : TeoAppException(message)
{
    public bool HasErrors => Errors.Count > 0;
    public Dictionary<string, string[]> Errors { get; } = new();

    public void WithError(string code, string[] values) =>
        Errors.Add(code, values);
}