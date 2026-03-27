using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagosWhatsappAPI.Data;
using PagosWhatsappAPI.DTOs;
using PagosWhatsappAPI.Services;

namespace PagosWhatsappAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthController(AppDbContext db, JwtService jwt)
    {
        _db = db; _jwt = jwt;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Username == req.Username && u.Activo);

        if (usuario == null ||
            !BCrypt.Net.BCrypt.Verify(req.Password, usuario.PasswordHash))
            return Unauthorized(ApiResponse<string>.Error(
                "Usuario o contraseña incorrectos"));

        var (token, expira) = _jwt.Generar(usuario);

        return Ok(ApiResponse<LoginResponse>.Ok(new LoginResponse
        {
            Token = token,
            Username = usuario.Username,
            Rol = usuario.Rol,
            Expiracion = expira
        }, "Login exitoso"));
    }
}