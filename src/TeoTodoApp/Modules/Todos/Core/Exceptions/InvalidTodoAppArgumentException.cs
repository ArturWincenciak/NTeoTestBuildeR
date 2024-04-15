using NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;

namespace NTeoTestBuildeR.Modules.Todos.Core.Exceptions;

public sealed class InvalidTodoAppArgumentException(string message)
    : AppArgumentException(message);