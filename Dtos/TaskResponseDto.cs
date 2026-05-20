using Taskify.Models;

namespace Taskify.Dtos;

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // DueDate em UTC — o frontend converte para o fuso local do utilizador
    public DateTime DueDate { get; set; }

    public int? EstimatedDuration { get; set; } // em minutos

    // Enums serializados como inteiros por defeito; ver nota abaixo
    public ItemStatus Status { get; set; }
    public TaskPriority? Priority { get; set; }

    public string? Category { get; set; }

    // Texto devolvido pelo LLM a explicar a sugestão de prioridade/categoria
    public string? LlmReasoning { get; set; }

    public DateTime CreatedAt { get; set; }

    // Calculado em runtime no modelo; exposto aqui para o frontend usar diretamente
    public bool IsOverdue { get; set; }
}
