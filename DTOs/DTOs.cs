namespace PagosWhatsappAPI.DTOs;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime Expiracion { get; set; }
}

public class RegistrarPagoRequest
{
    public string NumeroWhatsapp { get; set; } = string.Empty;
    public string? TipoDocumento { get; set; }
    public string? NroDocumento { get; set; }
    public string? NroContrato { get; set; }
    public int? NroCuota { get; set; }
    public string? Banco { get; set; }
    public string? NroOperacion { get; set; }
    public decimal? Monto { get; set; }
    public string? FechaOperacion { get; set; }
    public string? MensajeOriginal { get; set; }
}

public class ActualizarEstadoRequest
{
    public string Estado { get; set; } = string.Empty;
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string? msg = null)
        => new() { Success = true, Data = data, Message = msg };

    public static ApiResponse<T> Error(string msg)
        => new() { Success = false, Message = msg };
}