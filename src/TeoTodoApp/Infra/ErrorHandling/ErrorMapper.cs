using System.Net;
using Humanizer;
using NTeoTestBuildeR.Infra.ErrorHandling.Exceptions;

namespace NTeoTestBuildeR.Infra.ErrorHandling;

internal static class ErrorMapper
{
    public static ProblemDetails Map(Exception exception) =>
        exception switch
        {
            TeoAppException ex => MapTeoAppException(ex),
            _ => MapUnexpectedException()
        };

    private static ProblemDetails MapTeoAppException(Exception ex) =>
        ex switch
        {
            AppArgumentException argumentException => new(
                Type: ErrorType(ex),
                Title: "Request payload is not valid",
                Detail: argumentException.Message,
                Instance: string.Empty,
                Status: 400,
                Errors: argumentException.Errors,
                Extensions: new Dictionary<string, object>()),
            NotFoundException notFoundException => new(
                Type: ErrorType(ex),
                Title: "Resource does not exist",
                Detail: notFoundException.Message,
                Instance: string.Empty,
                Status: 404,
                Errors: new Dictionary<string, string[]>(),
                Extensions: new Dictionary<string, object>()),
            DomainException domainException => new(
                Type: ErrorType(ex),
                Title: "Invalid operation",
                Detail: domainException.Message,
                Instance: string.Empty,
                Status: 400,
                Errors: new Dictionary<string, string[]>(),
                Extensions: new Dictionary<string, object>()),
            InfraException infraException => new(
                Type: ErrorType(ex),
                Title: "Infrastructure error",
                Detail: infraException.Message,
                Instance: string.Empty,
                Status: 500,
                Errors: new Dictionary<string, string[]>(),
                Extensions: infraException.Context),
            AppNotImplementedException appNotImplementedException => new(
                Type: ErrorType(ex),
                Title: "Not implemented yet",
                Detail: appNotImplementedException.Message,
                Instance: string.Empty,
                Status: 500,
                Errors: new Dictionary<string, string[]>(),
                Extensions: new Dictionary<string, object>()),
            _ => MapUnexpectedException()
        };

    private static ProblemDetails MapUnexpectedException() =>
        new(
            Type: $"{DocUrl()}/internal-server-error.md",
            Title: "Internal server error",
            Status: (int) HttpStatusCode.InternalServerError,
            Detail: "Unexpected error occurred",
            Instance: string.Empty,
            Errors: new Dictionary<string, string[]>(),
            Extensions: new Dictionary<string, object>()
        );

    private static string DocUrl()
    {
        const string gitHubUrl = "https://github.com";
        const string repoUrl = $"{gitHubUrl}/ArturWincenciak/teo-test-builder";
        const string docUrl = $"{repoUrl}/doc/problem-details";
        return docUrl;
    }

    private static string ErrorCode(Exception ex) => ex
        .GetType().Name
        .Underscore()
        .Replace(oldValue: "_exception", string.Empty, StringComparison.OrdinalIgnoreCase)
        .Dasherize();

    private static string ErrorType(Exception ex) =>
        $"{DocUrl()}/{ErrorCode(ex)}.md";
}