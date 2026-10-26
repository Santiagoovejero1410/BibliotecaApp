using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using BibliotecaApi.Data;
using BibliotecaApi.Models;
using BibliotecaApi.Repositories;
using BibliotecaApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== 1) Base de datos: EF Core con SQL Server =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== 2) ASP.NET Identity: usuarios, contraseñas hasheadas y roles =====
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequiredLength = 6;              // reglas de contraseña más simples
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;           // no puede haber dos usuarios con el mismo email
    })
    .AddRoles<IdentityRole>()                             // habilita los roles (Admin, Socio)
    .AddEntityFrameworkStores<AppDbContext>();            // Identity guarda todo con nuestro DbContext

// ===== 3) Inyección de dependencias =====
// AddScoped: se crea una instancia nueva por cada pedido HTTP.
// "Cuando alguien pida la interfaz, entregale esta clase."
builder.Services.AddScoped<ILibroRepository, LibroRepository>();
builder.Services.AddScoped<IPrestamoRepository, PrestamoRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IPrestamoService, PrestamoService>();

builder.Services.AddControllers();

// ===== 4) Swagger (documentación) con botón "Authorize" para pegar el token =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Biblioteca API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegá acá el token que devuelve /api/auth/login"
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

// ===== 5) Autenticación con JWT: cómo validar el token que llega en "Authorization: Bearer ..." =====
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,                      // rechaza tokens vencidos
            ValidateIssuerSigningKey = true,              // verifica la firma con la clave secreta
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });

builder.Services.AddAuthorization();

// ===== 6) CORS: permite que el front (puerto 5173) llame a esta API (puerto 5080) =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Al arrancar: migraciones + roles + datos de ejemplo
using (var scope = app.Services.CreateScope())
{
    await DbInicial.CrearAsync(scope.ServiceProvider);
}

// ===== Pipeline (el ORDEN importa) =====
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Frontend");
app.UseAuthentication();      // ¿quién sos? (lee el token)
app.UseAuthorization();       // ¿tenés permiso? ([Authorize] y roles)
app.MapControllers();

app.Run();
