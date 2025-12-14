using Biometria.Domain.Entities;  //la entidad Usuario
using Biometria.Domain.Interfaces;   // La interfaz del servicio biométrico
using Biometria.Infrastructure.Data; // El DbContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Para trabajar con la BD
using ViewFaceCore.Model;           // Para AntiSpoofingStatus

namespace ApiReconocimientoFacial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Las 2 herramientas que necesitamos:
        private readonly IBiometricService _biometricService; // La IA
        private readonly AppDbContext _context;       // La Base de Datos

        // Inyección de dependencias (El "Constructor")
        public AuthController(IBiometricService biometricService, AppDbContext context)
        {
            _biometricService = biometricService;
            _context = context;
        }

        // ====================================================================
        // 1. REGISTRO: Sube foto + Nombre -> Guarda en SQL
        // ====================================================================
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(IFormFile foto, [FromForm] string nombre)
        {
            if (foto == null) return BadRequest("Falta la foto.");

            // A. Convertir la foto web a bytes
            byte[] fotoBytes;
            using (var ms = new MemoryStream())
            {
                await foto.CopyToAsync(ms);
                fotoBytes = ms.ToArray();
            }

            // B. La IA analiza la foto (DetectarYExtraerVector)
            // Fíjate que el controlador NO sabe cómo funciona la IA, solo pide el resultado.
            float[]? vector = _biometricService.DetectarYExtraerVector(fotoBytes);

            if (vector == null) return BadRequest("No se detectó ninguna cara en la imagen.");

            // C. Guardar en Base de Datos
            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                VectorFacial = vector, // ¡Magia! Se convierte a JSON automáticamente
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync(); // SQL INSERT

            return Ok(new { Mensaje = $"✅ Usuario '{nombre}' registrado correctamente." });
        }

        // ====================================================================
        // 2. LOGIN: Sube foto -> Busca coincidencia en SQL
        // ====================================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login(IFormFile foto)
        {
            if (foto == null) return BadRequest(new { Mensaje = "Falta la foto." });

            // A. Convertir a bytes
            byte[] fotoBytes;
            using (var ms = new MemoryStream())
            {
                await foto.CopyToAsync(ms);
                fotoBytes = ms.ToArray();
            }

            // --- CORRECCIÓN 1: Llamar una sola vez a verificar vida ---
            bool esReal = _biometricService.VerificarVida(fotoBytes);

            // Si NO es real (!esReal), devolvemos el error y TERMINAMOS aquí.
            if (!esReal)
            {
                return Ok(new
                {
                    Mensaje = "⚠️ ALERTA DE SEGURIDAD: Se detectó un intento de suplantación (foto o pantalla).",
                    Autorizado = false,
                    EsSpoofing = true
                });
            }

            // B. Sacar el vector de la persona
            float[]? vectorLogin = _biometricService.DetectarYExtraerVector(fotoBytes);

            if (vectorLogin == null)
                return BadRequest(new { Mensaje = "No se detectó ninguna cara frente a la cámara." });

            // C. BUSCAR EN LA BASE DE DATOS (El "Match")
            // Nota: Esto trae todos los usuarios a memoria. Si tienes mil usuarios está bien, 
            // si tienes un millón habrá que optimizarlo después.
            var usuariosDB = await _context.Usuarios.ToListAsync();

            Usuario? usuarioEncontrado = null;

            foreach (var user in usuariosDB)
            {
                if (user.VectorFacial == null) continue;

                if (_biometricService.SonLaMismaPersona(user.VectorFacial, vectorLogin))
                {
                    usuarioEncontrado = user;
                    break; // ¡Lo encontramos! Dejamos de buscar.
                }
            }

            // D. Responder
            if (usuarioEncontrado != null)
            {
                // --- CORRECCIÓN 2: Guardar el historial AQUÍ, cuando ya sabemos quién es ---
                var acceso = new Biometria.Domain.Entities.Acceso
                {
                    UsuarioId = usuarioEncontrado.Id,      // Ahora sí tiene valor
                    NombreUsuario = usuarioEncontrado.Nombre,
                    FechaHora = DateTime.Now,
                    FueExitoso = true,
                    NivelConfianza = 1.0f
                };

                _context.Accesos.Add(acceso);
                await _context.SaveChangesAsync();
                // -----------------------------------------------------------------------

                return Ok(new
                {
                    Mensaje = $"👋 ¡Bienvenido, {usuarioEncontrado.Nombre}!",
                    Autorizado = true,
                    EsSpoofing = false
                });
            }
            else
            {
                // Opcional: Podrías querer guardar también los intentos fallidos en el historial
                // con UsuarioId = null o un string "Desconocido".
                return Unauthorized(new
                {
                    Mensaje = "⛔ Cara real, pero usuario no registrado en el sistema.",
                    Autorizado = false
                });
            }
        }
    }
}