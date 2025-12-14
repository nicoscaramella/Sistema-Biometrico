
using Biometria.Domain.Interfaces;
using Biometria.Infrastructure.Data;
using Biometria.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using ViewFaceCore.Core;


// --- AGREGAR ESTO AL PRINCIPIO ---
// Esto obliga a la Web API a trabajar en la carpeta donde están los DLLs y Modelos
System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
// ---------------------------------

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod() // <--- ¡ESTO ES VITAL PARA QUE FUNCIONE DELETE!
              .AllowAnyHeader();
    });
});

builder.Services.AddScoped<FaceDetector>();
builder.Services.AddScoped<FaceLandmarker>();
builder.Services.AddScoped<FaceAntiSpoofing>();
builder.Services.AddScoped<FaceRecognizer>();

// Add services to the container.
builder.Services.AddScoped<IBiometricService, SeetaFaceService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseRouting();

// 2. Usar la política
app.UseCors("PermitirTodo");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
