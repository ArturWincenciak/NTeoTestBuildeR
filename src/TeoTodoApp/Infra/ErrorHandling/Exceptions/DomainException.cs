using JetBrains.Annotations;

namespace NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;

[PublicAPI]
public abstract class DomainException(string message)
    : TeoAppException(message);