<h1 align="center">Todo List backend</h1>

## Iniciando o projeto
Para rodar o projeto você precisa:
- .NET 7 SDK
- SQL Server (ou ajustar `CONNECTION_STRING`)

### Configuração de ambiente
As variáveis abaixo suportam autenticação JWT e autosave:

- `CONNECTION_STRING`
- `AUTH_JWT_ISSUER`
- `AUTH_JWT_AUDIENCE`
- `AUTH_JWT_KEY`
- `FEATURE_AUTOSAVE_ENABLED=true`

Também é possível configurar via `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "Jwt": {
    "Issuer": "TaskManager",
    "Audience": "TaskManagerClient",
    "Secret": "..."
  },
  "Features": {
    "AutosaveEnabled": true
  }
}
```

## Endpoints de autosave/kanban

### PATCH `/api/tasks/{id}`
Atualização parcial com controle de concorrência otimista.

Request:
```json
{
  "title": "Novo título",
  "description": "Atualização de autosave",
  "state": "InProgress",
  "order": 2,
  "estimateMinutes": 120,
  "spentMinutes": 45,
  "dueDate": "2026-03-10T14:00:00Z",
  "rowVersion": "AAAAAAAAB9E="
}
```

### POST `/api/tasks/{id}/move`
Movimenta card entre colunas e recalcula ordenação em transação.

Request:
```json
{
  "targetColumnId": 4,
  "targetOrder": 1,
  "targetState": "Done",
  "rowVersion": "AAAAAAAAB9E="
}
```

### PUT `/api/tasks/{id}/time`
Atualiza campos de tempo da tarefa.

### GET `/api/boards/{boardId}/tasks`
Lista tarefas de uma coluna/board com `state`, `order` e `rowVersion`.

## Concorrência
Se a `rowVersion` enviada divergir da atual, a API retorna `409 Conflict` com `ProblemDetails` e `currentRowVersion`.

## Architecture Diagram
See [docs/architecture.md](docs/architecture.md) for a high level flow of how the API handles requests.
