using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taskify.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public int? EstimatedDuration { get; set; } // em minutos

    public ItemStatus Status { get; set; } = ItemStatus.PorFazer;

    public TaskPriority? Priority { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    public string? LlmReasoning { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public bool IsOverdue =>
        DueDate < DateTime.UtcNow && Status != ItemStatus.Concluida;
}