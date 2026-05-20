using Taskify.Models;

namespace Taskify.Dtos;

public class TaskReprioritizeSuggestionDto
{
    // "id" corresponde ao campo devolvido pelo LLM no JSON
    public int Id { get; set; }
    public TaskPriority Priority { get; set; }
    public string Reasoning { get; set; } = string.Empty;
}
