using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Taskify;
using Taskify.Data;
using Taskify.Dtos;
using Taskify.Models;
using Taskify.Services;


// Configura serviços e middlewares antes de arrancar a app
var builder = WebApplication.CreateBuilder(args);

// Gera documentação OpenAPI (visível em /openapi/v1.json em desenvolvimento)
builder.Services.AddOpenApi();

// Regista o AppDbContext como serviço injetável, usando SQLite com a connection string do appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Regista o GroqService com um HttpClient gerido pelo framework
builder.Services.AddHttpClient<GroqService>();

// Serializa enums como strings ("PorFazer", "Alta") em vez de inteiros (0, 3)
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Constrói a app com os serviços registados acima
var app = builder.Build();

// Expõe o endpoint OpenAPI só em desenvolvimento (nunca em produção)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Serve index.html por defeito quando acedes a /
app.UseDefaultFiles();
// Serve os ficheiros estáticos da pasta wwwroot/
app.UseStaticFiles();

// Redireciona HTTP para HTTPS automaticamente
app.UseHttpsRedirection();

// Os endpoints da API vão ser mapeados aqui

// Agrupa todos os endpoints de tarefas sob o prefixo /tasks
var tasks = app.MapGroup("/tasks");

// Devolve todas as tarefas, ordenadas por prioridade descendente e depois por data limite
tasks.MapGet("/", async (AppDbContext db) =>
{
    var list = await db.Tasks
        .OrderByDescending(t => t.Priority)
        .ThenBy(t => t.DueDate)
        .ToListAsync();

    return list.Select(t => t.ToResponseDto());
});

// Devolve uma tarefa pelo Id; 404 se não existir
tasks.MapGet("/{id}", async (int id, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    return task is null ? Results.NotFound() : Results.Ok(task.ToResponseDto());
});

// Cria uma nova tarefa; devolve 201 com a localização do recurso criado
tasks.MapPost("/", async (CreateTaskDto dto, AppDbContext db) =>
{
    var task = new TaskItem
    {
        Title = dto.Title,
        Description = dto.Description,
        DueDate = dto.DueDate,
        EstimatedDuration = dto.EstimatedDuration,
        CreatedAt = DateTime.UtcNow
    };

    db.Tasks.Add(task);
    await db.SaveChangesAsync();

    return Results.Created($"/tasks/{task.Id}", task.ToResponseDto());
});

// Atualiza todos os campos editáveis de uma tarefa existente
tasks.MapPut("/{id}", async (int id, UpdateTaskDto dto, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();

    task.Title = dto.Title;
    task.Description = dto.Description;
    task.DueDate = dto.DueDate;
    task.EstimatedDuration = dto.EstimatedDuration;
    task.Status = dto.Status;
    task.Priority = dto.Priority;
    task.Category = dto.Category;

    await db.SaveChangesAsync();
    return Results.Ok(task.ToResponseDto());
});

// Apaga a tarefa permanentemente (hard delete)
tasks.MapDelete("/{id}", async (int id, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Chama o LLM para sugerir prioridade e categoria — não guarda nada, só devolve sugestão
tasks.MapPost("/{id}/suggest", async (int id, GroqService groq, AppDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    if (task is null) return Results.NotFound();

    var suggestion = await groq.SuggestAsync(task);
    return suggestion is null ? Results.Problem("O LLM não devolveu sugestão.") : Results.Ok(suggestion);
});

app.Run();
