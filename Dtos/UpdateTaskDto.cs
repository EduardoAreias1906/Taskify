using System.ComponentModel.DataAnnotations;
using Taskify.Models;

namespace Taskify.Dtos;

public class UpdateTaskDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }

    public int? EstimatedDuration { get; set; }

    // O utilizador pode mudar o estado (ex: arrastar no Kanban)
    public ItemStatus Status { get; set; }

    // Nullable — pode não ter prioridade ainda (tarefa sem análise LLM)
    public TaskPriority? Priority { get; set; }

    public string? Category { get; set; }
}
