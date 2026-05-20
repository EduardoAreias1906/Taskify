# Notas de desenvolvimento — Projeto 2

## [data] — porque um segundo projeto
A razão de pretender fazer um segundo Projeto, após o Notify é que achei interessante, gostei de trabalhar nele, quero também explorar outras abordagens e outros temas, este tema parece-me mais complexo que o anterior, com muita coisa que pode vir a ser mudada, mesmo tendo passado muito tempo no planeamento.


## [data] — stack obrigatório vs escolhido
**Obrigatório pelo recrutador:** .NET, C#, REST, SQLite ou in-memory.

**Escolhi:**
- SQLite: Já trabalhei com o in-memory no projeto anterior, quero explorar outras alternativas.
- Minimal API: É mais comum no .NET 9, os endpoints são definidos no Program.cs. Achei a escolha correta para uma API desta dimensão (pequena). 
- EF Core: Mantive o EF Core como ORM, pelas mesmas razões que no Notify: é o standard em .NET moderno e dá-me CRUD funcional rapidamente, com migrações que facilitam evolução do schema. Considerei alternativas que conhecia da pesquisa para o projeto 1 — Dapper (escreves SQL à mão, mais performante) e ADO.NET puro (controlo total mas muito verboso). Para o âmbito deste exercício, a produtividade do EF Core compensa qualquer perda de performance."
- Groq como LLM provider: Foi o mesmo utilizado no Notify. Gratuito sem precisas de colocar cartão de crédito. API é compatível com OpenAI, facilita a troca de provider mais tarde se quiser.
- Validação: Para validação dos DTOs usei DataAnnotations, os atributos standard do .NET. Cobre bem as regras do meu modelo (campos obrigatórios, comprimentos, intervalos). Considerei FluentValidation, que é mais expressivo e suporta regras complexas, mas para este projeto as regras são simples e a dependência extra não se justifica. 


## [data] — produto

### Modelo de tarefa
- Title, Description, DueDate (obrigatórios), EstimatedDuration, Status, Priority, Category, LlmReasoning, IsOverdue (apenas para frontend)

### Prioridade
- 5 níveis: Crítica, Alta, Média, Baixa, Mínima
- Enum interno + label em PT-PT

### Estados
- Por fazer, Em curso, Concluída

### LLM
- Sugere prioridade e categoria; utilizador confirma ou altera
- Só corre quando o utilizador clica num botão (nunca automaticamente)
- Botão "estimar duração com LLM" quando o utilizador não sabe
- Botão "re-priorizar lista" para recalcular tudo

### Categorias
- Lista evolutiva
- Sempre PT-PT, primeira letra maiúscula

### Tarefas atrasadas
- Marcação visual imediata
- Prioridade só muda se o utilizador clicar em re-priorizar

### UI
- Kanban com 3 colunas (uma por estado)
- Ordem dentro de cada coluna: por prioridade descendente
- Drag-and-drop entre colunas muda o estado
- Fluxo de criação em 2 etapas: preenche → LLM analisa → utilizador confirma

### Apagar
- Hard delete


## [data] — decisões técnicas durante implementação

- **IsOverdue calculado em runtime:** propriedade com `[NotMapped]` no modelo — nunca persiste na BD, nunca fica desfasada. Exposta no `TaskResponseDto` para o frontend usar diretamente.
- **Enums como strings:** configurei `JsonStringEnumConverter` via `ConfigureHttpJsonOptions` (forma correta para Minimal API) — a API devolve `"PorFazer"` em vez de `0`, mais legível para o frontend JS.
- **`MapGroup("/tasks")`:** agrupa todos os endpoints de tarefas com prefixo comum — organização sem abstração desnecessária.
- **`TaskMappings.cs`:** classe estática com método de extensão `ToResponseDto()` para converter `TaskItem` em `TaskResponseDto` — evita repetição sem overhead de AutoMapper.
- **`ItemStatus` em vez de `TaskStatus`:** evita conflito com `System.Threading.Tasks.TaskStatus` que existe no .NET base.


## Uso de IA
(Vais preenchendo à medida que avanças, como fizeste no Notify)

## Dificuldades e como as resolvi
(Vais preenchendo)

## Para o README final
(Vais preenchendo)
