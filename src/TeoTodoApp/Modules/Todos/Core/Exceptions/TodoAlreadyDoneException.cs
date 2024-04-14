using NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;

namespace NTeoTestBuildeR.Modules.Todos.Core.Exceptions;

public sealed class TodoAlreadyDoneException(string message)
    : DomainException(message);