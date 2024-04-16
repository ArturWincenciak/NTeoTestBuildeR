using NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;

namespace NTeoTestBuildeR.Modules.Todos.Core.Exceptions;

public sealed class CalendarClientUriException()
    : InfraException("Calendar client base address is not configured");