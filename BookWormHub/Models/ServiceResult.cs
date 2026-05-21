namespace BookWormHub.Models;

public class ServiceResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
    public Dictionary<string, string> ValidationErrors { get; set; } = new();

    public static ServiceResult Ok(string? message = null) =>
        new() { Success = true, Message = message };

    public static ServiceResult Fail(string error) =>
        new() { Success = false, Error = error };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public new static ServiceResult<T> Fail(string error) =>
        new() { Success = false, Error = error };
}
