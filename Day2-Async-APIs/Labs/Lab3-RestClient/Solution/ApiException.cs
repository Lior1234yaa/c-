using System.Net;

namespace Day2.Lab3;

/// <summary>שגיאה שהשרת החזיר (סטטוס לא צפוי) — כולל ההודעה מה-body.</summary>
public class ApiException(HttpStatusCode statusCode, string message) : Exception($"{(int)statusCode} {statusCode}: {message}")
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
