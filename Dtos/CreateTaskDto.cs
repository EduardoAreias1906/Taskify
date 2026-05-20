using System.ComponentModel.DataAnnotations;

namespace Taskify.Dtos;

public class CreateTaskDto
{
    // Título obrigatório, limitado a 200 caracteres para não sobrecarregar a BD
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    // Descrição obrigatória — o LLM usa este campo para sugerir prioridade e categoria
    [Required]
    public string Description { get; set; } = string.Empty;

    // Data limite obrigatória — usada para calcular IsOverdue em runtime
    [Required]
    public DateTime DueDate { get; set; }

    // Opcional — o utilizador pode não saber; existe botão para o LLM estimar
    public int? EstimatedDuration { get; set; } // em minutos
}
