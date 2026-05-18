using API.GestionComercial;
using API.GestionComercial.Middleware;
using Application.Interfaces;
using Application.Mappings;
using AutoMapper;
using Infrastructure.Persistence;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplication();

var config = new MapperConfiguration(cfg =>
{
    cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
}, LoggerFactory.Create(b => b.AddConsole()));

builder.Services.AddSingleton<IMapper>(config.CreateMapper());
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPaisService, PaisService>();
builder.Services.AddScoped<IMonedaService, MonedaService>();
builder.Services.AddScoped<IUnidadMedidaService, UnidadMedidaService>();
builder.Services.AddScoped<IModuloSistemaService, ModuloSistemaService>();
builder.Services.AddScoped<IParametroSistemaService, ParametroSistemaService>();
builder.Services.AddScoped<IPaisValidatorService, PaisValidatorService>();
builder.Services.AddScoped<IMonedaValidatorService, MonedaValidatorService>();
builder.Services.AddScoped<IUnidadMedidaValidatorService, UnidadMedidaValidatorService>();
builder.Services.AddScoped<IModuloSistemaValidatorService, ModuloSistemaValidatorService>();
builder.Services.AddScoped<IParametroSistemaValidatorService, ParametroSistemaValidatorService>();

// Organizacion
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<IEmpresaValidatorService, EmpresaValidatorService>();
builder.Services.AddScoped<ISucursalService, SucursalService>();
builder.Services.AddScoped<ISucursalValidatorService, SucursalValidatorService>();
builder.Services.AddScoped<IAlmacenService, AlmacenService>();
builder.Services.AddScoped<IAlmacenValidatorService, AlmacenValidatorService>();

// Sprint 3 - Catalogo Fiscal
builder.Services.AddScoped<ITipoImpuestoService, TipoImpuestoService>();
builder.Services.AddScoped<ITipoComprobanteService, TipoComprobanteService>();
builder.Services.AddScoped<ISerieDocumentoService, SerieDocumentoService>();

// Sprint 4 - Producto Enriquecido (CategoriaProducto, MarcaProducto)
builder.Services.AddScoped<ICategoriaProductoService, CategoriaProductoService>();
builder.Services.AddScoped<ICategoriaProductoValidatorService, CategoriaProductoValidatorService>();
builder.Services.AddScoped<IMarcaProductoService, MarcaProductoService>();
builder.Services.AddScoped<IMarcaProductoValidatorService, MarcaProductoValidatorService>();

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AngularPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
