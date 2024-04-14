using JetBrains.Annotations;

namespace NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;

[PublicAPI]
public abstract class NotFoundException(string message)
    : TeoAppException(message);