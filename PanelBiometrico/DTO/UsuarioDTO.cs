namespace PanelBiometrico.DTO
{

    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }
}
