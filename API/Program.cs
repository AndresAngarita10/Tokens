using Microsoft.EntityFrameworkCore;
using API.Extensions;
using Persistencia;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJwt(builder.Configuration); //--Inyeccion servicios JWT Extensions
builder.Services.AddHttpContextAccessor();//--- parece qie se necesita con JWT
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
app.UseAuthentication();//-- Va junto al JWT, va antes de autorizacion siempre
app.UseAuthorization();

app.MapControllers();

app.Run();
