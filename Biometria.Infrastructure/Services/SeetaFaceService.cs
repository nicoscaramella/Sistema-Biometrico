using Biometria.Domain.Interfaces;
using SkiaSharp;
using System;
using System.IO;
using System.Threading;
using ViewFaceCore.Core;
using ViewFaceCore.Model;

namespace Biometria.Infrastructure.Services
{
    public class SeetaFaceService : IBiometricService
    {
        private readonly FaceDetector _faceDetector;
        private FaceRecognizer _faceRecognizer;
        private readonly FaceLandmarker _faceLandmarker;
        private FaceAntiSpoofing _faceAntiSpoofing;

        public SeetaFaceService(
            FaceDetector detector,
            FaceRecognizer recognizer,
            FaceLandmarker landmarker,
            FaceAntiSpoofing antiSpoofing)
        {
            _faceDetector = detector;
            _faceRecognizer = recognizer;
            _faceLandmarker = landmarker;
            _faceAntiSpoofing = antiSpoofing;
        }

        public float[]? DetectarYExtraerVector(byte[] fotoBytes)
        {
            using var memoryStream = new MemoryStream(fotoBytes);
            using var bitmap = SKBitmap.Decode(memoryStream);

            if (bitmap == null) throw new Exception("Imagen inválida.");

            float[]? resultadoVector = null;
            Exception? error = null;

            var hilo = new Thread(() =>
            {
                try
                {
                    var imagenIA = bitmap.ToFaceImageSeguro();

                    using var detector = new FaceDetector();
                    var caras = detector.Detect(imagenIA);

                    if (caras.Length > 0)
                    {
                        var info = caras[0]; // La cara principal

                        using var marcador = new FaceLandmarker();
                        var puntos = marcador.Mark(imagenIA, info); // Necesitamos los puntos (ojos/nariz)

                        // =========================================================
                        // 🛡️ AQUÍ ESTÁ EL ESCUDO ANTI-SPOOFING
                        // =========================================================
                        using var antiFraude = new FaceAntiSpoofing();  

                        // La IA analiza la textura de la piel
                        var estado = antiFraude.AntiSpoofing(imagenIA, info, puntos);

                        // AntiSpoofingStatus puede ser: Real, Spoof, Fuzzy (Dudoso) o Detecting
                        if (estado.Status == AntiSpoofingStatus.Real)
                        {
                            // SOLO SI ES REAL, SACAMOS EL VECTOR
                            using var reconocedor = new FaceRecognizer();
                            resultadoVector = reconocedor.Extract(imagenIA, puntos);
                        }
                        else
                        {
                            // Si es Spoof (foto/celular) o Fuzzy, lo ignoramos
                            Console.WriteLine($"⚠️ ALERTA DE SEGURIDAD: Intento de fraude detectado. Estado: {estado}");
                            // Dejamos resultadoVector en null
                        }
                        // =========================================================
                    }
                }
                catch (Exception ex) { error = ex; }
            }, 10 * 1024 * 1024);

            hilo.Start();
            hilo.Join();

            if (error != null) throw error;
            return resultadoVector;
        }
        public bool SonLaMismaPersona(float[] vector1, float[] vector2)
        {
            using var reconocedor = new FaceRecognizer();

            // CORRECCIÓN 3: Usamos los nombres de los parámetros (vector1, vector2)
            float similitud = reconocedor.Compare(vector1, vector2);

            // CORRECCIÓN 4: Quitamos la variable 'porcentaje' que sobraba
            return similitud > 0.62;
        }

        public bool VerificarVida(byte[] fotoBytes)
        {
            try
            {
                using SKBitmap bitmap = SKBitmap.Decode(fotoBytes);
                if (bitmap == null) return false;

                // USAMOS TU MÉTODO EXTENSIÓN AQUÍ
                using var faceImage = bitmap.ToFaceImageSeguro();

                // Detectar
                var faceInfos = _faceDetector.Detect(faceImage);
                if (faceInfos == null || faceInfos.Length == 0) return false;

                var info = faceInfos[0];

                // Puntos (Necesarios para AntiSpoofing)
                var points = _faceLandmarker.Mark(faceImage, info);

                // Predecir
                var result = _faceAntiSpoofing.AntiSpoofing(faceImage, info, points);

                // Retornar true/false
                return result.Status == AntiSpoofingStatus.Real;
            }
            catch
            {
                return false;
            }
        }
    }

}

    

    // =========================================================
    // EL CIRUJANO (Tienes que incluir esto aquí o en otro archivo)
    // =========================================================
    public static class MisExtensiones
    {
        public static FaceImage ToFaceImageSeguro(this SKBitmap bitmap)
        {
            if (bitmap.ColorType != SKColorType.Bgra8888)
            {
                var info = new SKImageInfo(bitmap.Width, bitmap.Height, SKColorType.Bgra8888);
                var nuevo = new SKBitmap(info);
                using (var c = new SKCanvas(nuevo)) c.DrawBitmap(bitmap, 0, 0);
                bitmap = nuevo;
            }

            int pixels = bitmap.Width * bitmap.Height;
            byte[] original = bitmap.Bytes;
            byte[] rawBgr = new byte[pixels * 3];

            for (int i = 0; i < pixels; i++)
            {
                int k = i * 4;
                int j = i * 3;
                rawBgr[j] = original[k];
                rawBgr[j + 1] = original[k + 1];
                rawBgr[j + 2] = original[k + 2];
            }
            return new FaceImage(bitmap.Width, bitmap.Height, 3, rawBgr);
        }
    }