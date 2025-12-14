using Biometria.Domain.Interfaces;
using Biometria.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiReconocimientoFacial.Controllers
{
    // 1. IMPORTANTE: Definimos la ruta base aquí para no repetirla en cada método
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBiometricService _biometricService;

        // 2. CORRECCIÓN: Inyectamos AMBOS servicios en el constructor
        public UsuariosController(AppDbContext context, IBiometricService biometricService)
        {
            _context = context;
            _biometricService = biometricService; // <--- ¡Esto faltaba!
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.FechaRegistro
                })
                .ToListAsync();
            return Ok(usuarios);
        }

        // GET: api/usuarios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.FechaRegistro
                })
                .FirstOrDefaultAsync();

            if (usuario == null) return NotFound(new { Mensaje = "Usuario no encontrado." });

            return Ok(usuario);
        }

        // DELETE: api/usuarios/5
        // Ahora sí funcionará porque hereda "api/usuarios" de la clase + "{id}" de aquí
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound(new { Mensaje = "Usuario no encontrado." });

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { Mensaje = $"✅ Usuario '{usuario.Nombre}' eliminado correctamente." });
        }

        // PUT: api/usuarios/5/foto
        [HttpPut("{id}/foto")]
        public async Task<IActionResult> ActualizarFoto(int id, IFormFile nuevaFoto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound(new { Mensaje = "Usuario no encontrado." });

            if (nuevaFoto == null) return BadRequest("Falta la nueva foto.");

            using var memoryStream = new MemoryStream();
            await nuevaFoto.CopyToAsync(memoryStream);
            byte[] bytes = memoryStream.ToArray();

            // Esto ya no dará error porque inyectamos el servicio en el constructor
            var nuevoVector = _biometricService.DetectarYExtraerVector(bytes);

            if (nuevoVector == null) return BadRequest("No se detectó ninguna cara en la nueva imagen.");

            usuario.VectorFacial = nuevoVector;

            await _context.SaveChangesAsync();
            return Ok(new { Mensaje = $"✅ Foto del usuario '{usuario.Nombre}' actualizada correctamente." });
        }
    }
}