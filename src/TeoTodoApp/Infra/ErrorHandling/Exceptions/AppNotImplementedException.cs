using JetBrains.Annotations;

namespace NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;

[PublicAPI]
public sealed class AppNotImplementedException(string message)
    : TeoAppException(message);