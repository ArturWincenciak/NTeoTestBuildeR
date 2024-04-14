using JetBrains.Annotations;

namespace NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;

[PublicAPI]
public abstract class InfraException(string message)
    : TeoAppException(message)
{
    public Dictionary<string, object> Context { get; } = new();

    public void WithContext(string code, string[] values) =>
        Context.Add(code, values);
}