using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TurnosApi.Data;
using TurnosApi.Middleware;
using TurnosApi.Repositories;
using TurnosApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ============ 1) BASE DE DATOS (EF Core + SQL Server) ============
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============ 2) INYECCIÓN DE DEPENDENCIAS ============
// AddScoped = se crea una instancia nueva por cada request HTTP.
// Registramos "interfaz -> clase concreta": cuando alguien pida IAuthService, ASP.NET entrega AuthService.
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
builder.Services.AddScoped<ITurnoRepository, TurnoRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IServicioService, ServicioService>();
builder.Services.AddScoped<ITurnoService, TurnoService>();

builder.Services.AddControllers();

// ============ 3) SWAGGER (documentación) con botón "Authorize" para pegar el JWT ============
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Turnos API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegá acá el token JWT que devuelve /api/auth/login"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ============ 4) AUTENTICACIÓN JWT ============
// Le dice a ASP.NET cómo VALIDAR el token que llega en el header "Authorization: Bearer xxx"
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,                 // Rechaza tokens vencidos
            ValidateIssuerSigningKey = true,         // Verifica la firma con la clave secreta
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ============ 5) CORS ============
// El navegador bloquea pedidos entre orígenes distintos (front :5173 -> back :5080).
// Con esta política le decimos al back que acepte pedidos del front.
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Aplica las migraciones pendientes al arrancar y carga datos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

// ============ PIPELINE (el ORDEN importa) ============
app.UseMiddleware<ExceptionHandlingMiddleware>();   // 1º: atrapa cualquier error
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Frontend");                            // CORS antes de autenticación
app.UseAuthentication();                            // ¿Quién sos? (lee el token)
app.UseAuthorization();                             // ¿Tenés permiso? ([Authorize], roles)
app.MapControllers();

app.Run();
