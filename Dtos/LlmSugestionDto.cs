using Taskify.Models;

namespace Taskify.Dtos;

public class LlmSuggestionDto
{
    // Prioridade sugerida pelo LLM — o utilizador confirma ou altera antes de guardar
    public TaskPriority Priority { get; set; }

    // Categoria sugerida em PT-PT
    public string Category { get; set; } = string.Empty;

    // Explicação do raciocínio do LLM para ajudar o utilizador a decidir
    public string Reasoning { get; set; } = string.Empty;
}
