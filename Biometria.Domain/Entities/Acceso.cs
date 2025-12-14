using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometria.Domain.Entities
{
    public class Acceso
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; } // Clave foránea
        public string NombreUsuario { get; set; } // Guardamos el nombre por si borran el usuario después
        public DateTime FechaHora { get; set; }
        public bool FueExitoso { get; set; }
        public float NivelConfianza { get; set; } // Opcional: Qué tanto se parecia (Score)
    }
}
