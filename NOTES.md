# Notas de desenvolvimento — Projeto 2

## [data] — porque um segundo projeto
A razão de pretender fazer um segundo Projeto, após o Notify é que achei interessante, gostei de trabalhar nele, quero também explorar outras abordagens e outros temas, este tema parece-me mais complexo que o anterior, com muita coisa que pode vir a ser mudada, mesmo tendo passado muito tempo no planeamento.


## [data] — stack obrigatório vs escolhido
**Obrigatório pelo recrutador:** .NET, C#, REST, SQLite ou in-memory.

**Escolhi:**
- SQLite: Já trabalhei com o in-memory no projeto anterior, quero explorar outras alternativas.
- Minimal API: É mais comum no .NET 9, os endpoints são definidos no Program.cs. Achei a escolha correta para uma API desta dimensão (pequena).
- EF Core: Mantive o EF Core como ORM, pelas mesmas razões que no Notify: é o standard em .NET moderno e dá-me CRUD funcional rapidamente, com migrações que facilitam evolução do schema. Considerei alternativas que conhecia da pesquisa para o projeto 1 — Dapper (escreves SQL à mão, mais performante) e ADO.NET puro (controlo total mas muito verboso). Para o âmbito deste exercício, a produtividade do EF Core compensa qualquer perda de performance.
- Groq como LLM provider: Foi o mesmo utilizado no Notify. Gratuito sem precisares de colocar cartão de crédito. API compatível com OpenAI, facilita a troca de provider mais tarde se quiser.
- Validação: Para validação dos DTOs usei DataAnnotations, os atributos standard do .NET. Cobre bem as regras do meu modelo (campos obrigatórios, comprimentos, intervalos). Considerei FluentValidation, que é mais expressivo e suporta regras complexas, mas para este projeto as regras são simples e a dependência extra não se justifica.


## [data] — produto

### Modelo de tarefa
- Title, Description, DueDate (obrigatórios), EstimatedDuration, Status, Priority, Category, LlmReasoning, IsOverdue (calculado em runtime, não persiste)

### Prioridade
- 5 níveis: Crítica, Alta, Média, Baixa, Mínima
- Enum interno + label em PT-PT

### Estados
- Por fazer, Em curso, Concluída

### LLM
- Sugere prioridade e categoria para uma tarefa; utilizador confirma ou altera (fluxo de criação em 2 passos)
- Botão "Re-priorizar" reavalia todas as tarefas em conjunto, tendo em conta o contexto global — o LLM evita inflacionar prioridades quando há muitas tarefas competindo
- LLM só corre quando o utilizador clica num botão (nunca automaticamente)

### Categorias
- Lista evolutiva
- Sempre PT-PT, primeira letra maiúscula

### Tarefas atrasadas
- Marcação visual imediata (badge "Atrasada")
- Prioridade só muda se o utilizador clicar em re-priorizar

### UI
- Kanban com 3 colunas (uma por estado): Por fazer, Em curso, Concluída
- Ordem dentro de cada coluna: por prioridade descendente, depois por data limite
- Drag-and-drop entre colunas muda o estado (PUT /tasks/{id})
- Fluxo de criação em 2 etapas: preenche → LLM analisa → utilizador confirma
- Edição de tarefa: botão ✎ em cada card abre modal pré-preenchido
- Frontend em HTML + CSS + JS puro, servido como ficheiros estáticos via wwwroot/

### Apagar
- Hard delete


## [data] — decisões técnicas durante implementação

- **IsOverdue calculado em runtime:** propriedade com `[NotMapped]` no modelo — nunca persiste na BD, nunca fica desfasada. Exposta no `TaskResponseDto` para o frontend usar diretamente.
- **Enums como strings:** configurei `JsonStringEnumConverter` via `ConfigureHttpJsonOptions` (forma correta para Minimal API) — a API devolve `"PorFazer"` em vez de `0`, mais legível para o frontend JS.
- **`MapGroup("/tasks")`:** agrupa todos os endpoints de tarefas com prefixo comum — organização sem abstração desnecessária.
- **`TaskMappings.cs`:** classe estática com método de extensão `ToResponseDto()` para converter `TaskItem` em `TaskResponseDto` — evita repetição sem overhead de AutoMapper.
- **`ItemStatus` em vez de `TaskStatus`:** evita conflito com `System.Threading.Tasks.TaskStatus` que existe no .NET base.
- **`db.Database.EnsureCreated()` em vez de `Migrate()`:** `Migrate()` não estava a criar o ficheiro `.db` (a connection string estava em falta no `appsettings.json`). `EnsureCreated()` cria a schema diretamente do modelo, sem depender do sistema de migrações — mais simples e robusto para desenvolvimento.
- **`UseHttpsRedirection` desativado em desenvolvimento:** o perfil HTTP corre só na porta 5230 sem HTTPS configurado; o redirect causava falhas nos pedidos `fetch` do frontend. Condicionado a `!IsDevelopment()`.
- **`$$"""` em vez de `$"""`nos prompts ao LLM:** os prompts incluem JSON de exemplo com `{` e `}` literais. Em raw string literals interpoladas, `$"""` não suporta `{{` como escape; com `$$"""` as interpolações passam a ser `{{expr}}` e as chavetas literais ficam como `{` — sem conflito.
- **`Id` em vez de `TaskId` no `TaskReprioritizeSuggestionDto`:** o LLM devolve `"id"` no JSON mas o DTO tinha `TaskId`; `PropertyNameCaseInsensitive` não faz match entre nomes diferentes, só entre cases diferentes. Renomear para `Id` resolveu a deserialização.


## Dificuldades e como as resolvi

- **"no such table: Tasks":** a connection string nunca tinha sido adicionada ao `appsettings.json`. `GetConnectionString("DefaultConnection")` devolvia `null`, o SQLite usava uma base de dados in-memory que desaparecia entre pedidos.
- **`UseHttpsRedirection` a quebrar o frontend:** os pedidos `fetch` feitos a partir do HTML (HTTP:5230) recebiam um redirect 307 para HTTPS, que não estava configurado em desenvolvimento. Solução: condicionar o middleware a produção.
- **Raw string literal com `{` literal:** ao usar `$"""` com `{{` para escapar chavetas no exemplo JSON do prompt, o compilador deu erro ("not enough '$' characters"). Causa: em raw string literals, `{{` não é uma sequência de escape — é um erro de sintaxe. Solução: usar `$$"""` com `{{expr}}` para interpolações.
- **Re-priorizar mostrava `#0` em vez do título:** o `taskId` ficava sempre a zero porque o LLM devolve `"id"` mas o DTO tinha `TaskId` — nomes diferentes não fazem match mesmo com `PropertyNameCaseInsensitive`. Renomear a propriedade para `Id` resolveu.


## Uso de IA
Ferramenta: Claude Code (claude-sonnet-4-6), via extensão VS Code.

O modo de trabalho foi deliberadamente colaborativo: eu escrevia o código, o Claude explicava conceitos, mostrava exemplos em blocos de código e sugeria o próximo passo. Não gerei código automaticamente — copiei ou reescrevi à minha maneira o que o Claude mostrava.

**O que o Claude fez:**
- Explicou cada peça antes de mostrar código (modelo, DTOs, DbContext, GroqService, endpoints)
- Alertou para o conflito de nomes `TaskStatus` vs `System.Threading.Tasks.TaskStatus`
- Explicou a diferença entre `$"""` e `$$"""` em raw string literals e porquê `{{` causava erro
- Explicou `EnsureCreated()` vs `Migrate()` e quando usar cada um
- Diagnosticou e explicou 4 bugs: connection string em falta, `UseHttpsRedirection` em desenvolvimento, `{{` em raw string literals, e mismatch `TaskId`/`id` na deserialização do LLM
- Escreveu diretamente o `wwwroot/index.html` (pedi explicitamente), incluindo Kanban, drag-and-drop, modal de criação, edição e re-priorização
- Fez edições pontuais a pedido: `appsettings.json`, `EnsureCreated()` no `Program.cs`, rename `TaskId` → `Id`
