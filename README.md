# 🛡️ Sistema de Control de Acceso Biométrico con IA y Anti-Spoofing

![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![SeetaFace6](https://img.shields.io/badge/AI-SeetaFace6-blue?style=for-the-badge)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)

> **Un sistema de autenticación biométrica completo que utiliza Inteligencia Artificial local para detectar rostros, extraer vectores biométricos y prevenir suplantación de identidad (Anti-Spoofing).**

---

## 🚀 Descripción General

Este proyecto resuelve la necesidad de autenticación segura sin contraseñas. A diferencia de soluciones tradicionales, este sistema no depende de APIs en la nube (como Azure Face o AWS Rekognition); todo el procesamiento de IA ocurre **On-Premise (localmente)** en el servidor, garantizando privacidad y velocidad.

El núcleo del sistema utiliza **Redes Neuronales Convolucionales (CNN)** a través del motor **SeetaFace6** para convertir un rostro humano en un vector matemático único. Además, implementa un algoritmo de **Liveness Detection** para asegurar que quien está frente a la cámara es una persona real y no una foto o video desde un celular.

## ✨ Funcionalidades Clave

### 👤 Usuario Final
* **Login Facial Instantáneo:** Acceso mediante reconocimiento en tiempo real.
* **Anti-Spoofing (Prueba de Vida):** El sistema rechaza intentos de acceso si detecta una fotografía o pantalla frente a la cámara.
* **Optimización de Red:** Las imágenes se comprimen y redimensionan en el cliente (Blazor) antes de enviarse, reduciendo el consumo de ancho de banda.

### 🛠️ Administrador (Panel Protegido)
* **Gestión de Usuarios:** Alta y baja de personal autorizado.
* **Auditoría de Seguridad:** Historial detallado de accesos (exitosos y fallidos/spoofing) con fecha y hora.
* **Dashboard Seguro:** Acceso restringido mediante credenciales administrativas.

## 🏗️ Arquitectura y Tecnologías

El proyecto sigue una **Arquitectura Limpia (Clean Architecture)** separada en capas:

* **Frontend:** Blazor WebAssembly (C# en el navegador, Interop con JS para manejo de cámara).
* **Backend:** ASP.NET Core 8 Web API.
* **Inteligencia Artificial:**
    * Librería: **ViewFaceCore** (Wrapper de .NET para SeetaFace6).
    * Capacidades: Detección facial, Alineación (Landmarks), Extracción de Vectores (Feature Extraction) y Anti-Spoofing.
* **Persistencia:** Entity Framework Core + SQL Server (LocalDB).

## ⚙️ Instalación y Puesta en Marcha

Sigue estos pasos para ejecutar el proyecto en tu entorno local:

1. **Clonar el repositorio:**
   ```bash
   git clone [https://github.com/](https://github.com/)[TU_USUARIO]/Sistema-Biometrico-AntiSpoofing.git
   
2. **Base de Datos**
    El proyecto utiliza SQL Server LocalDB. Asegúrate de tenerlo instalado (viene con Visual Studio). Ejecuta las migraciones para crear la base de datos automáticamente:
```bash
    cd ApiReconocimientoFacial
    dotnet ef database update --project ../Biometria.Infrastructure
```
3. **Ejecución**

Abre la solución SistemaBiometrico.sln en Visual Studio 2022.

Establece ApiReconocimientoFacial y PanelBiometrico para que inicien juntos (Botón derecho en Solución -> Propiedades -> Proyectos de inicio múltiples).

Presiona F5 o "Iniciar".

4. **Credenciales de Admin**
   Para acceder al panel de gestión (/usuarios), utiliza la contraseña por defecto: admin123.




   Desarrollado por **Nicolás Scaramella**.
   Contacto: scaramellanicolas5@gmail.com

