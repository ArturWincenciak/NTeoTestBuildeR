using NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;

namespace NTeoTestBuildeR.Modules.Todos.Core.Exceptions;

public sealed class CreateCalendarEventException(string message)
    : InfraException(message);