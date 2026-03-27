namespace PagosWhatsappAPI.Models;

public class PagoRecibido
{
    public int Id { get; set; }
    public string NumeroWhatsapp { get; set; } = string.Empty;
    public string? TipoDocumento { get; set; }
    public string? NroDocumento { get; set; }
    public string? NroContrato { get; set; }
    public int? NroCuota { get; set; }
    public string? Banco { get; set; }
    public string? NroOperacion { get; set; }
    public decimal? Monto { get; set; }
    public string? FechaOperacion { get; set; }
    public string Estado { get; set; } = "PENDIENTE";
    public string? MensajeOriginal { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public DateTime? FechaActualizacion { get; set; }
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
}