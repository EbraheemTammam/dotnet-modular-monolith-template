namespace Base.Responses;

public record Response
{
    protected static bool ValidateStatusCode(int statusCode, bool succeeded)
    {
        if (statusCode < 200 || statusCode > 599)
        {
            throw new ArgumentOutOfRangeException(nameof(statusCode), "Status code should be between 200 and 599");
        }
        if (succeeded && statusCode >= 400)
        {
            throw new ArgumentOutOfRangeException(nameof(statusCode), "Status code should be less than 400 for successful responses");
        }
        if (!succeeded && statusCode < 400)
        {
            throw new ArgumentOutOfRangeException(nameof(statusCode), "Status code should be greater than 400 for unsuccessful responses");
        }
        return true;
    }
    public int StatusCode { get; set; }
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    protected Response(int statusCode, bool succeeded, string? message = null)
    {
        ValidateStatusCode(statusCode, succeeded);
        StatusCode = statusCode;
        Succeeded = succeeded;
        Message = message;
    }
    public static Response Success(int statusCode = 200) => new(statusCode, true);
    public static Response Fail(string? message = null, int statusCode = 400) => new(statusCode, false, message);
    public static Response NotFound(Guid id, string resource, string idField = "id") =>
        new(404, false, $"{resource} with {idField} {id} Not Found");
    public static Response BadRequest(string message) => new(400, false, message);
    public static Response InternalServerError = new(500, false, "Something went wrong");
    public static Response UnAuthorized = new(401, false, "Email or password is incorrect");
    public static Response Forbidden = new(403, false, "Permission denied");
}

public sealed record Response<TData> : Response
{
    public TData? Data { get; set; }
    private Response(int statusCode, TData? data, bool succeeded, string? message = null) : base(statusCode, succeeded, message)
    {
        ValidateStatusCode(statusCode, succeeded);
        Data = data;
    }
    public static Response<TData> Success(TData data, int statusCode = 200) => new(statusCode, data, true);
}
