using System.Net;

namespace CreditFlow.API.Core.Errors;

public abstract class AppException : Exception
{
    protected AppException(ErrorDefinition error, HttpStatusCode statusCode)
        : this(error.Code, error.Message, statusCode)
    {
    }

    protected AppException(string code, string message, HttpStatusCode statusCode)
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }

    public string Code { get; }
    public HttpStatusCode StatusCode { get; }
}

public sealed class RequestValidationException(ErrorDefinition error)
    : AppException(error, HttpStatusCode.BadRequest);

public sealed class UnauthorizedAppException(ErrorDefinition error)
    : AppException(error, HttpStatusCode.Unauthorized);

public sealed class ResourceNotFoundException : AppException
{
    public ResourceNotFoundException(ErrorDefinition error)
        : base(error, HttpStatusCode.NotFound) { }

    public ResourceNotFoundException(string code, string message)
        : base(code, message, HttpStatusCode.NotFound) { }
}

public sealed class BusinessRuleException : AppException
{
    public BusinessRuleException(ErrorDefinition error)
        : base(error, HttpStatusCode.UnprocessableEntity) { }

    public BusinessRuleException(string code, string message)
        : base(code, message, HttpStatusCode.UnprocessableEntity) { }
}

public sealed class ResourceConflictException : AppException
{
    public ResourceConflictException(ErrorDefinition error)
        : base(error, HttpStatusCode.Conflict) { }

    public ResourceConflictException(string code, string message)
        : base(code, message, HttpStatusCode.Conflict) { }
}
