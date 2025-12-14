namespace PanelBiometrico.DTO
{
    public class AccesoDTO
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public DateTime FechaHora { get; set; }
        public bool FueExitoso { get; set; }
    }
}
