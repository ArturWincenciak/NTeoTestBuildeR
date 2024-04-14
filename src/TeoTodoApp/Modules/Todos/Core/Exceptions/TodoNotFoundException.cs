using NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;

namespace NTeoTestBuildeR.Modules.Todos.Core.Exceptions;

public sealed class TodoNotFoundException(string message)
    : NotFoundException(message);