namespace BOMManagement.Application.Common.Results;

public class Result : IResult
{
    public Result(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public Result(bool success)
    {
        Success = success;
        Message = string.Empty;
    }

    public bool Success { get; }
    public string Message { get; }
}
