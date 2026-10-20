namespace TurnosApi.Exceptions;

// Excepción propia con un código HTTP. Los Services la lanzan ("throw new NotFoundException(...)")
// y el ExceptionHandlingMiddleware la convierte en una respuesta HTTP limpia.
public class AppException : Exception
{
    public int StatusCode { get; }
    public AppException(int statusCode, string message) : base(message) => StatusCode = statusCode;
}

public class NotFoundException : AppException        // 404: no existe
{
    public NotFoundException(string message) : base(404, message) { }
}

public class BadRequestException : AppException      // 400: datos inválidos
{
    public BadRequestException(string message) : base(400, message) { }
}

public class ConflictException : AppException        // 409: conflicto (ej: horario ocupado, email repetido)
{
    public ConflictException(string message) : base(409, message) { }
}

public class UnauthorizedAppException : AppException // 401: credenciales incorrectas
{
    public UnauthorizedAppException(string message) : base(401, message) { }
}

public class ForbiddenException : AppException       // 403: no tenés permiso para esto
{
    public ForbiddenException(string message) : base(403, message) { }
}
