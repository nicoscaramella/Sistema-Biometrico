namespace PanelBiometrico.Services
{
  
        public class AuthState
        {
            public bool EstaLogueado { get; private set; } = false;

            public void Login()
            {
                EstaLogueado = true;
                NotifyStateChanged();
            }

            public void Logout()
            {
                EstaLogueado = false;
                NotifyStateChanged();
            }

            // Evento para avisar al Layout que la pantalla cambió
            public event Action? OnChange;
            private void NotifyStateChanged() => OnChange?.Invoke();
        }
}