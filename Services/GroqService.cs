using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Taskify.Dtos;
using Taskify.Models;

namespace Taskify.Services;

public class GroqService(HttpClient httpClient, IConfiguration config)
{
    private const string Model = "llama-3.3-70b-versatile";
    private const string ApiUrl = "https://api.groq.com/openai/v1/chat/completions";

    public async Task<LlmSuggestionDto?> SuggestAsync(TaskItem task)
    {
        // Lê a chave dos user-secrets (nunca commitada)
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", config["Groq:ApiKey"]);

        // Prompt estruturado — os valores de priority têm de corresponder exatamente ao enum
        // Pré-calcula para evitar interpolação aninhada dentro do raw string
        var durationText = task.EstimatedDuration.HasValue
            ? $"{task.EstimatedDuration} minutos"
            : "não definida";

        // $$""" permite usar { literal no JSON de exemplo sem escaping
        var prompt = $$"""
            Analisa esta tarefa e responde APENAS com JSON válido, sem texto adicional.

            Tarefa:
            - Título: {{task.Title}}
            - Descrição: {{task.Description}}
            - Data limite: {{task.DueDate:dd/MM/yyyy}}
            - Duração estimada: {{durationText}}

            Responde com este formato exato:
            {
            "priority": "Critica" | "Alta" | "Media" | "Baixa" | "Minima",
            "category": "<categoria em português, primeira letra maiúscula>",
            "reasoning": "<explicação curta em português>"
            }
            """;

        var requestBody = new
        {
            model = Model,
            messages = new[]
            {
                new { role = "system", content = "És um assistente de gestão de tarefas. Respondes sempre em JSON válido." },
                new { role = "user", content = prompt }
            },
            response_format = new { type = "json_object" },
            temperature = 0.3 // valor baixo para respostas mais consistentes e menos criativas
        };

        var body = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.PostAsync(ApiUrl, body);
        response.EnsureSuccessStatusCode();

        // Navega pela estrutura da resposta OpenAI-compatible para extrair o conteúdo
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (content is null) return null;

        // Deserializa o JSON devolvido pelo LLM, com suporte a enums como strings
        return JsonSerializer.Deserialize<LlmSuggestionDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });
    }
}
