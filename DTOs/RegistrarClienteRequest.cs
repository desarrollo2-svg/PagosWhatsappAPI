namespace PagosWhatsappAPI.DTOs;
public class RegistrarClienteRequest
{
    public string TipoDocumento { get; set; } = "DNI";
    public string NroDocumento { get; set; } = string.Empty;
    public string? Nombre { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public string NumeroWhatsapp { get; set; } = string.Empty;
}