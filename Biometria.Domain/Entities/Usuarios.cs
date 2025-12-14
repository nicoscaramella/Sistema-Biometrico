using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Biometria.Domain.Entities
{
        public class Usuario
        {
            [Key] // Llave primaria (Auto-incremental)
            public int Id { get; set; }

            [Required]
            public string Nombre { get; set; } = string.Empty;

            public string? Correo { get; set; }

            // --- TRUCO: GUARDAR VECTOR COMO TEXTO ---

            // 1. Esta propiedad SE GUARDA en SQL (NVARCHAR)
            // [JsonIgnore] para que no se envíe al frontend ni ensucie el JSON de respuesta
            [System.Text.Json.Serialization.JsonIgnore]
            public string VectorFacialJson { get; set; } = "[]";

            // 2. Esta propiedad NO SE GUARDA en SQL ([NotMapped])
            // Es un "puente" que convierte el Texto a float[] automáticamente cuando lo usas
            [NotMapped]
            public float[]? VectorFacial
            {
                get
                {
                    return string.IsNullOrEmpty(VectorFacialJson)
                        ? null
                        : JsonSerializer.Deserialize<float[]>(VectorFacialJson);
                }
                set
                {
                    // Si el valor es null, guardamos "[]", si no, guardamos el JSON
                    VectorFacialJson = value == null ? "[]" : JsonSerializer.Serialize(value);
                }
            }

            public DateTime FechaRegistro { get; set; } = DateTime.Now;
        }
}

