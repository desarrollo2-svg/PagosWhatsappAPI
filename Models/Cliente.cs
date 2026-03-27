namespace PagosWhatsappAPI.Models;

public class Cliente
{
    public int Id { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string NroDocumento { get; set; } = string.Empty;
    public string? Nombre { get; set; }
    public string? Telefono { get; set; }
    public string NumeroWhatsapp { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;
    public ICollection<PagoRecibido> Pagos { get; set; } = new List<PagoRecibido>();
}