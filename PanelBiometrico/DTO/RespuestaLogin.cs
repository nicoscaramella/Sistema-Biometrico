namespace PanelBiometrico.DTO
{
    public class RespuestaLogin
    {
        public string Mensaje { get; set; } = string.Empty;
        public bool Autorizado { get; set; }
        public bool EsSpoofing { get; set; }
    }
}
