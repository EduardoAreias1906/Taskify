# Instruções para o Claude Code

Este ficheiro orienta o agente de IA usado no desenvolvimento deste projeto.
Ver NOTES.md para o registo cronológico de decisões e dificuldades.

## Sobre o projeto

API REST de gestão de tarefas com sugestões de prioridade e categoria por LLM. Segundo desafio técnico para uma entrevista na Sparksoft, depois do projeto Notify (API de notas com tagging e summary por LLM). A escolha de fazer um segundo projeto foi minha, quis explorar um uso diferente do LLM (priorização estruturada em vez de geração livre).

## Stack obrigatório (definido pelo recrutador)

- **.NET 9**
- **C#**
- **Endpoints REST**
- **Persistência:** SQLite (escolhi este em vez de in-memory para ter ficheiro persistente entre execuções)

## Stack escolhido por mim

- **Estilo da API:** Minimal API (consistente com projeto 1, mas considerei alternativa de Controllers para experimentar)
- **ORM:** EF Core (code-first com migrações)
- **LLM provider:** Groq (modelo Llama 3.3 70B), API compatível com OpenAI
- **Frontend (se houver tempo):** HTML + CSS + JS puro, servido como ficheiros estáticos pela API (`wwwroot/`)
- **Validação:** atributos de DataAnnotations (sem FluentValidation, para manter simples)
- **Testes:** (decidir se faço, dependendo do tempo restante)

## Como quero trabalhar contigo

**Eu escrevo o código. Tu ensinas e ajudas.**

- Não escrevas código nos meus ficheiros sem eu pedir explicitamente. Mostra-me o que faria num bloco de código na resposta, eu copio (ou reescrevo à minha maneira) e meto no ficheiro.
- Quando eu pedir para escreveres num ficheiro, faz só a peça mais pequena possível. Uma classe, um endpoint, um método.
- Explica antes de mostrar código: o que vai fazer, porquê assim, que alternativas existem.
- Pergunta antes de assumir. Se eu não fui claro num requisito, pergunta. Não inventes.
- Não corras `dotnet run`, `dotnet build`, `git commit` ou comandos no terminal sem eu pedir. Eu faço.
- Não atualizes o NOTES.md por mim. Podes sugerir o que vale a pena registar. O registo é meu.
- Se eu te mostrar código meu para revisão, foca-te em apontar 2-3 coisas, não 10.

**O que podes fazer livremente:**
- Ler qualquer ficheiro do projeto para perceberes contexto.
- Listar pastas, ver estrutura.
- Explicar conceitos, mostrar exemplos em blocos de código (sem escrever no ficheiro).
- Sugerir o próximo passo.

## Decisões de produto já tomadas

(Ver NOTES.md para detalhe e justificação de cada uma)

- Utilizador único, sem autenticação
- Tarefa: Title, Description, DueDate (obrigatórios), EstimatedDuration, Status, Priority, Category, LlmReasoning
- 5 níveis de prioridade: Crítica, Alta, Média, Baixa, Mínima (enum interno + label PT-PT)
- 3 estados: Por fazer, Em curso, Concluída
- LLM sugere prioridade e categoria; utilizador confirma ou altera (opção B)
- LLM só corre quando o utilizador clica num botão; nunca automaticamente
- Categorias: lista evolutiva, sempre PT-PT com primeira letra maiúscula
- Tarefas atrasadas: marcação visual, sem alteração automática de prioridade
- Hard delete

## Convenções

- Namespace raiz: (decidir consoante o nome do projeto)
- Pastas: Models/, Data/, Dtos/, Services/, Migrations/
- Sempre `DateTime.UtcNow`
- Commits pequenos e frequentes, mensagens em inglês
- Chave do Groq via `dotnet user-secrets`, nunca commitada

## Restrições

- Não introduzir bibliotecas externas sem me consultares primeiro.
- Não fazer abstrações prematuras num projeto deste tamanho.
- Não fazer commits automaticamente. Eu reviso e faço commit.
- Respeitar o stack obrigatório acima — não sugerir mudanças que o violem.