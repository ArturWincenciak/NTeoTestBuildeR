namespace NTeoTestBuildeR.Infra.ErrorHandling;

public record ProblemDetails(
    string Type,
    int Status,
    string Title,
    string Detail,
    string Instance,
    IDictionary<string, string[]> Errors,
    IDictionary<string, object> Extensions);