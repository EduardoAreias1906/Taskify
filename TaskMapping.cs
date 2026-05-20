using Taskify.Dtos;
using Taskify.Models;

namespace Taskify;

// Métodos de extensão para converter entre modelo e DTO
public static class TaskMappings
{
    public static TaskResponseDto ToResponseDto(this TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        DueDate = task.DueDate,
        EstimatedDuration = task.EstimatedDuration,
        Status = task.Status,
        Priority = task.Priority,
        Category = task.Category,
        LlmReasoning = task.LlmReasoning,
        CreatedAt = task.CreatedAt,
        // IsOverdue é calculado pela propriedade do modelo, não vem da BD
        IsOverdue = task.IsOverdue
    };
}
