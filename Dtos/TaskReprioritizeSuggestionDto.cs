using Taskify.Models;

namespace Taskify.Dtos;

public class TaskReprioritizeSuggestionDto
{
    // Id da tarefa a que esta sugestão se refere
    public int TaskId { get; set; }
    public TaskPriority Priority { get; set; }
    public string Reasoning { get; set; } = string.Empty;
}
