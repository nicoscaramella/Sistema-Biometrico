using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometria.Domain.Interfaces
{
    public interface IBiometricService
    {
        float[]? DetectarYExtraerVector(byte[] fotoBytes);

        bool SonLaMismaPersona(float[] vector1, float[] vector2);

        bool VerificarVida(byte[] fotoBytes);
    }
}
