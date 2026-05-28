using Clinica.Developer.Repository;
using Clinica.Developer.Service;
using MongoDB.Driver;
using Psicología.Developer.Repository;
using Psicología.Developer.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Configuración de Mongo...
var connectionString = builder.Configuration.GetConnectionString("MongoDb");
var databaseName = builder.Configuration.GetSection("ConnectionStrings:DatabaseName").Value;
builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

// 2. Registro del Repositorio
builder.Services.AddScoped<PsicologiaRepository>(sp =>
    new PsicologiaRepository(sp.GetRequiredService<IMongoClient>(), databaseName));

builder.Services.AddScoped<FisioterapiaRepository>(sp =>
    new FisioterapiaRepository(sp.GetRequiredService<IMongoClient>(), databaseName));

builder.Services.AddScoped<OdontologoRepository>(sp =>
    new OdontologoRepository(sp.GetRequiredService<IMongoClient>(), databaseName));

// =======================================================================
builder.Services.AddScoped<PsicologiaService>();
builder.Services.AddScoped<FisioterapiaService>();
builder.Services.AddScoped<OdontologoService>();
// =======================================================================

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SEGURIDAD: CORS ----------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// --------------------------------------------------------------------------------------------

var app = builder.Build();

// Habilitar la política de CORS que registramos arriba
app.UseCors("PermitirTodo");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
