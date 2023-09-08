using Microsoft.EntityFrameworkCore;
using API.Extensions;
using Persistencia;
using Microsoft.AspNetCore.Authorization;
using API.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();//--- parece qie se necesita con JWT
builder.Services.AddAplicacionServices();// -- no olvidar ponerlo tambien
builder.Services.AddJwt(builder.Configuration); //--Inyeccion servicios JWT Extensions
builder.Services.AddAuthorization(opt =>  // --- Esto es del JWT
{
    opt.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new GlobalVerbRoleRequirement())
            .Build();
});

builder.Services.AddDbContext<TokensContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("ConexMysql");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
//app.UseAuthentication();//-- Va junto al JWT, va despues de autorizacion siempre

app.MapControllers();

app.Run();
