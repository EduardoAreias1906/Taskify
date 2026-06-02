# Taskify

API REST de gestão de tarefas com sugestões de prioridade e categoria por LLM. Segundo desafio técnico desenvolvido para processo de entrevista na Sparksoft.

## Stack

| Camada | Tecnologia |
|--------|-----------|
| Runtime | .NET 9, C# |
| API | Minimal API |
| ORM | EF Core 10 (code-first) |
| Base de dados | SQLite |
| LLM | Groq — Llama 3.3 70B (API compatível com OpenAI) |
| Frontend | HTML + CSS + JS puro (servido como ficheiros estáticos) |

## Funcionalidades

- **CRUD de tarefas** — criar, listar, editar e apagar tarefas
- **Kanban** — 3 colunas (Por fazer, Em curso, Concluída) com drag-and-drop para mudar estado
- **Sugestão por LLM** — ao criar uma tarefa, o utilizador pode pedir ao LLM uma sugestão de prioridade e categoria; confirma ou ignora antes de guardar
- **Re-priorização global** — botão que envia todas as tarefas ao LLM para reavaliação conjunta de prioridades, com prévia antes de confirmar
- **Marcação de tarefas atrasadas** — calculada em runtime, sem coluna extra na BD
- **Edição de tarefas** — modal pré-preenchido com todos os campos editáveis

## Modelo de tarefa

| Campo | Tipo | Notas |
|-------|------|-------|
| Title | string | Obrigatório |
| Description | string | Obrigatório |
| DueDate | DateTime | Obrigatório, UTC |
| EstimatedDuration | int? | Em minutos |
| Status | enum | PorFazer, EmCurso, Concluida |
| Priority | enum? | Critica, Alta, Media, Baixa, Minima |
| Category | string? | PT-PT, primeira letra maiúscula |
| LlmReasoning | string? | Preenchido pelo LLM |
| IsOverdue | bool | Calculado em runtime (`DueDate < UtcNow && Status != Concluida`) |

## Como correr

**Pré-requisitos:** .NET 10 SDK, conta Groq com chave de API.

```bash
# Clonar o repositório
git clone <url>
cd Taskify

# Configurar a chave do Groq (nunca commitada)
dotnet user-secrets set "Groq:ApiKey" "sk-..."

# Correr
dotnet run
```

Abre `http://localhost:5230` no browser.

A base de dados SQLite (`taskify.db`) é criada automaticamente na primeira execução.

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/tasks` | Lista todas as tarefas (ordenadas por prioridade e data) |
| GET | `/tasks/{id}` | Devolve uma tarefa |
| POST | `/tasks` | Cria uma tarefa |
| PUT | `/tasks/{id}` | Atualiza uma tarefa |
| DELETE | `/tasks/{id}` | Apaga uma tarefa (hard delete) |
| POST | `/tasks/{id}/suggest` | LLM sugere prioridade e categoria para a tarefa |
| POST | `/tasks/reprioritize` | LLM reavalia prioridades de todas as tarefas em conjunto |

## Notas

- Utilizador único, sem autenticação
- A chave do Groq é gerida via `dotnet user-secrets` e nunca entra no repositório
- Os enums são serializados como strings na API (`"Alta"`, `"PorFazer"`)
