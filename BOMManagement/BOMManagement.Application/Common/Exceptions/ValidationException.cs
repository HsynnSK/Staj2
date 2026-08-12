namespace BOMManagement.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(IDictionary<string, string[]> failures)
        : base("One or more validation failures have occurred.")
    {
        Errors = failures;
    }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public IDictionary<string, string[]> Errors { get; }
}
