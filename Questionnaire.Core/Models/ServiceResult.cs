namespace Questionnaire.Core.Models;

public class ServiceResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public static ServiceResult SuccessResult(string? message = null)
    {
        return new ServiceResult()
        {
            Success = true,
            Message = message
        };
    }

    public static ServiceResult ErrorResult(string? message = null)
    {
        return new ServiceResult()
        {
            Success = false,
            Message = message
        };
    }
}

public class ServiceResult<T>:ServiceResult
{
    public T? Result { get; set; }
    
    public static ServiceResult<T> SuccessResult(T result, string? message = null)
    {
        return new ServiceResult<T>()
        {
            Success = true,
            Message = message,
            Result = result
        };
    }

    public new static ServiceResult<T> ErrorResult(string? message = null)
    {
        return new ServiceResult<T>()
        {
            Success = false,
            Message = message
        };
    }
}