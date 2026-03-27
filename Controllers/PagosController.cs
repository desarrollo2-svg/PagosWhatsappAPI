using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagosWhatsappAPI.Data;
using PagosWhatsappAPI.DTOs;
using PagosWhatsappAPI.Models;

namespace PagosWhatsappAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PagosController : ControllerBase
{
    private readonly AppDbContext _db;
    public PagosController(AppDbContext db) => _db = db;

    // n8n llama a este endpoint cuando el cliente confirma el pago
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarPagoRequest req)
    {
        // Buscar cliente existente o crear uno nuevo automáticamente
        Cliente? cliente = null;
        if (!string.IsNullOrEmpty(req.NroDocumento))
        {
            cliente = await _db.Clientes.FirstOrDefaultAsync(
                c => c.NroDocumento == req.NroDocumento);

            if (cliente == null)
            {
                cliente = new Cliente
                {
                    TipoDocumento = req.TipoDocumento ?? "DNI",
                    NroDocumento = req.NroDocumento,
                    NumeroWhatsapp = req.NumeroWhatsapp,
                    FechaRegistro = DateTime.Now
                };
                _db.Clientes.Add(cliente);
                await _db.SaveChangesAsync();
            }
            else if (cliente.NumeroWhatsapp != req.NumeroWhatsapp)
            {
                cliente.NumeroWhatsapp = req.NumeroWhatsapp;
            }
        }

        var pago = new PagoRecibido
        {
            NumeroWhatsapp = req.NumeroWhatsapp,
            TipoDocumento = req.TipoDocumento,
            NroDocumento = req.NroDocumento,
            NroContrato = req.NroContrato,
            NroCuota = req.NroCuota,
            Banco = req.Banco,
            NroOperacion = req.NroOperacion,
            Monto = req.Monto,
            FechaOperacion = req.FechaOperacion,
            MensajeOriginal = req.MensajeOriginal,
            Estado = "PENDIENTE",
            FechaRegistro = DateTime.Now,
            ClienteId = cliente?.Id
        };
        _db.PagosRecibidos.Add(pago);
        await _db.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(new
        {
            pago.Id,
            pago.Estado,
            pago.FechaRegistro,
            clienteId = cliente?.Id,
            clienteNuevo = cliente != null
        }, "Pago registrado correctamente"));
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? estado,
        [FromQuery] string? nroContrato,
        [FromQuery] string? wa,
        [FromQuery] int pag = 1,
        [FromQuery] int x = 20)
    {
        var q = _db.PagosRecibidos.Include(p => p.Cliente).AsQueryable();
        if (!string.IsNullOrEmpty(estado))
            q = q.Where(p => p.Estado == estado.ToUpper());
        if (!string.IsNullOrEmpty(nroContrato))
            q = q.Where(p => p.NroContrato == nroContrato);
        if (!string.IsNullOrEmpty(wa))
            q = q.Where(p => p.NumeroWhatsapp == wa);

        var total = await q.CountAsync();
        var datos = await q.OrderByDescending(p => p.FechaRegistro)
            .Skip((pag - 1) * x).Take(x).ToListAsync();

        return Ok(new { success = true, total, pagina = pag, data = datos });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var p = await _db.PagosRecibidos.Include(p => p.Cliente)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (p == null) return NotFound(ApiResponse<string>.Error("No encontrado"));
        return Ok(ApiResponse<PagoRecibido>.Ok(p));
    }

    [HttpPatch("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(
        int id, [FromBody] ActualizarEstadoRequest req)
    {
        var validos = new[] { "PENDIENTE", "VALIDADO", "RECHAZADO" };
        if (!validos.Contains(req.Estado.ToUpper()))
            return BadRequest(ApiResponse<string>.Error("Estado inválido"));

        var p = await _db.PagosRecibidos.FindAsync(id);
        if (p == null) return NotFound(ApiResponse<string>.Error("No encontrado"));

        p.Estado = req.Estado.ToUpper();
        p.FechaActualizacion = DateTime.Now;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<string>.Ok(p.Estado, "Estado actualizado"));
    }
}