using Microsoft.EntityFrameworkCore;
using Taskify.Data;

// Configura serviços e middlewares antes de arrancar a app
var builder = WebApplication.CreateBuilder(args);

// Gera documentação OpenAPI (visível em /openapi/v1.json em desenvolvimento)
builder.Services.AddOpenApi();

// Regista o AppDbContext como serviço injetável, usando SQLite com a connection string do appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Constrói a app com os serviços registados acima
var app = builder.Build();

// Expõe o endpoint OpenAPI só em desenvolvimento (nunca em produção)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Redireciona HTTP para HTTPS automaticamente
app.UseHttpsRedirection();

// Os endpoints da API vão ser mapeados aqui

app.Run();
