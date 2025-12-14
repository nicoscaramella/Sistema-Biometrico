using Biometria.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiReconocimientoFacial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccesosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccesosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUltimosAccesos()
        {
            // Consultamos la base de datos
            var historial = await _context.Accesos
                .OrderByDescending(a => a.FechaHora) // Los más recientes primero
                .Take(50) // Limitamos a 50 para que sea rápido
                .Select(a => new
                {
                    a.Id,
                    a.NombreUsuario,
                    a.FechaHora,
                    a.FueExitoso
                })
                .ToListAsync();

            return Ok(historial);
        }
    }
}