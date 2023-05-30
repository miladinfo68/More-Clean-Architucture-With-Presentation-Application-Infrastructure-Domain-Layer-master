using CleanArch.Common.Constants;

namespace CleanArch.Common.Exceptions;

public class ResponseNullException : Exception
{
    public int StatusCode { get; } = 400;

    public ResponseNullException(string message = DefaultErrorMessages.NullException) : base(message)
    {
    }
}

public class BadRequestException : Exception
{
    public int StatusCode { get; } = 400; 

    public BadRequestException(string message=DefaultErrorMessages.BadRequest) : base(message)
    {
    }
}

public class AuthorizationException : Exception
{
    public int StatusCode { get; } = 401; // Unauthorized

    public AuthorizationException(string message=DefaultErrorMessages.UnAuthorized) : base(message)
    {
    }
}

public class AccessDenyException : Exception
{
    public int StatusCode { get; } = 403; // Unauthorized

    public AccessDenyException(string message=DefaultErrorMessages.Forbidden) : base(message)
    {
    }
}

public class NotFoundException : Exception
{
    public int StatusCode { get; } = 404; 

    public NotFoundException(string message=DefaultErrorMessages.NotFound) : base(message)
    {
    }
}

public class RequestTimeoutException : Exception
{
    public int StatusCode { get; } = 408; // Request Timeout

    public RequestTimeoutException(string message=DefaultErrorMessages.ReqTimeOuut) : base(message)
    {
    }
}

public class CancellationException : Exception
{
    public int StatusCode { get; } = 408; 

    public CancellationException(string message=DefaultErrorMessages.ReqCancled) : base(message)
    {
    }
}


public class UnKnownException : Exception
{
    public int StatusCode { get; } = 500; 

    public UnKnownException(string message = DefaultErrorMessages.UnKnownError) : base(message)
    {
    }
}


public class SqlServerException : Exception
{
    public int StatusCode { get; } = 503; // Service Unavailable

    public SqlServerException(string message=DefaultErrorMessages.SqlError) : base(message)
    {
    }
}

public class NetworkException : Exception
{
    public int StatusCode { get; } = 503; // Service Unavailable

    public NetworkException(string message=DefaultErrorMessages.NetworkError) : base(message)
    {
    }
}


