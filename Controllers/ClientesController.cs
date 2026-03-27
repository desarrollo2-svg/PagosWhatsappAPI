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
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _db;
    public ClientesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? buscar,
        [FromQuery] int pag = 1,
        [FromQuery] int x = 20)
    {
        var q = _db.Clientes.Include(c => c.Pagos)
            .Where(c => c.Activo).AsQueryable();

        if (!string.IsNullOrEmpty(buscar))
            q = q.Where(c =>
                c.NroDocumento.Contains(buscar) ||
                c.NumeroWhatsapp.Contains(buscar) ||
                (c.Nombre != null && c.Nombre.Contains(buscar)));

        var total = await q.CountAsync();
        var datos = await q.OrderByDescending(c => c.FechaRegistro)
            .Skip((pag - 1) * x).Take(x)
            .Select(c => new {
                c.Id,
                c.TipoDocumento,
                c.NroDocumento,
                c.Nombre,
                c.Telefono,
                c.NumeroWhatsapp,
                c.FechaRegistro,
                TotalPagos = c.Pagos.Count
            }).ToListAsync();

        return Ok(new { success = true, total, data = datos });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var c = await _db.Clientes.Include(c => c.Pagos)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (c == null) return NotFound(ApiResponse<string>.Error("No encontrado"));

        return Ok(ApiResponse<object>.Ok(new
        {
            c.Id,
            c.TipoDocumento,
            c.NroDocumento,
            c.Nombre,
            c.Telefono,
            c.NumeroWhatsapp,
            c.FechaRegistro,
            TotalPagos = c.Pagos.Count,
            Pagos = c.Pagos.OrderByDescending(p => p.FechaRegistro)
                .Select(p => new {
                    p.Id,
                    p.NroContrato,
                    p.NroCuota,
                    p.Banco,
                    p.Monto,
                    p.Estado,
                    p.FechaRegistro
                })
        }));
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Actualizar(int id,
        [FromBody] Dictionary<string, string> campos)
    {
        var c = await _db.Clientes.FindAsync(id);
        if (c == null) return NotFound(ApiResponse<string>.Error("No encontrado"));

        if (campos.TryGetValue("nombre", out var nombre)) c.Nombre = nombre;
        if (campos.TryGetValue("telefono", out var tel)) c.Telefono = tel;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<string>.Ok("OK", "Cliente actualizado"));
    }
}