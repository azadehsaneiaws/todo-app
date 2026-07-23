# TODO App
<img width="1620" height="855" alt="image" src="https://github.com/user-attachments/assets/909bc0bc-567d-4cfb-a81b-c1974f600566" />

A TODO list application with an ASP.NET Core (.NET 10) REST API backend and an
Angular frontend. Users can view their list, add items, mark them complete and
delete them. I kept the Data  in memory on the backend for the lifetime of the
process, so no database setup is required.

## Requirements

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) or later (backend)
- [Node.js 20+](https://nodejs.org) and npm (frontend)

Check what you have installed:

```bash
dotnet --version
node --version
```

## Running the app

The app has two parts. Run each in its own terminal.

**1. Backend API** (from the repository root):

```bash
dotnet run --project src/TodoApp.Api
```

Starts on `http://localhost:5089`. No steps beyond restoring NuGet packages,
which `dotnet run` does automatically on first build.

**2. Frontend** (from the repository root):

```bash
cd todo-web
npm install
npm start
```

Starts on `http://localhost:4200` — open that in a browser. The dev server
proxies `/api` calls to the backend (see `todo-web/proxy.conf.json`), so both
parts run same-origin with no CORS setup needed. Make sure the backend is
running first.

## Running the tests

Backend:

```bash
dotnet test
```

Runs domain unit tests, service tests, and HTTP integration tests that spin up
the API in memory and exercise the real request pipeline.

Frontend:

```bash
cd todo-web
npm test
```

## Trying it out

The quickest way is the included `src/TodoApp.Api/TodoApp.Api.http` file, which
works out of the box in Visual Studio and the VS Code REST Client extension.

Or with `curl`:

```bash
# Add a todo
curl -X POST http://localhost:5089/api/todos \
  -H "Content-Type: application/json" \
  -d '{ "title": "Buy milk" }'

# List todos
curl http://localhost:5089/api/todos
```

## API reference

| Method   | Route             | Description              | Success        |
|----------|-------------------|--------------------------|----------------|
| `GET`    | `/api/todos`      | List all todos           | `200 OK`       |
| `GET`    | `/api/todos/{id}` | Get one todo by id       | `200 OK`       |
| `POST`   | `/api/todos`      | Add a todo               | `201 Created`  |
| `PUT`    | `/api/todos/{id}` | Rename / toggle complete | `200 OK`       |
| `DELETE` | `/api/todos/{id}` | Delete a todo            | `204 No Content` |

A missing id returns `404 Not Found`. An empty or missing `title` returns
`400 Bad Request` with a validation problem-details body.

A todo looks like this on the wire:

```json
{
  "id": "8a76c634-801f-4c17-b56d-28317f3d4bca",
  "title": "Buy milk",
  "isCompleted": false,
  "createdAt": "2026-07-22T09:15:00.123456+00:00"
}
```

## Project structure

```
src/TodoApp.Api            ASP.NET Core Web API (backend)
  Controllers/     HTTP endpoints (thin, delegate to the service)
  Services/        Application logic and DTO mapping
  Repositories/    Storage abstraction + in-memory implementation
  Models/          Domain model (TodoItem)
  Contracts/       Request/response DTOs
tests/TodoApp.Api.Tests    Backend tests
  Domain/          Domain model unit tests
  Services/        Service-level tests
  Api/             End-to-end HTTP tests
todo-web                   Angular frontend
  src/app/
    app.ts         Root component (list / add / toggle / delete)
    todo.service.ts  HttpClient wrapper for the REST API
    todo.model.ts  Todo type shared across the app
  proxy.conf.json  Forwards /api calls to the backend in dev
```

## Design notes

A few decisions I made that worth calling out:

- **Layering (Controller → Service → Repository).** Controllers only handle
  HTTP concerns; the service holds the use cases; the repository owns storage.
  Each layer depends on an interface, not a concrete type, so the pieces are
  independently testable and easy to change.

- **Repository pattern over an in-memory store.** The brief allows in-memory
  data, so `InMemoryTodoRepository` uses a `ConcurrentDictionary` (thread-safe
  for concurrent requests) and is registered as a singleton. Because everything
  depends on `ITodoRepository`, swapping in an Entity Framework Core / SQL
  Server implementation later is a single-class change with no impact on the
  service or controller.

- **DTOs separate from the domain model.** The API's request/response contracts
  are their own types, so the public shape can evolve without leaking or
  constraining the domain model. The `TodoItem` model guards its own invariants
  (a todo always has a non-empty, trimmed title).

- **Deliberately not over-engineered.** For a service this size, patterns like
  CQRS/MediatR or a multi-project "clean architecture" split would add
  ceremony without value. The layering above gives the same testability and
  separation with far less friction.

- **Synchronous by design.** The store is in-memory, so the code is honestly
  synchronous rather than wrapping everything in `Task` to look async. When a
  real datastore is introduced, the repository interface is the place those
  methods become genuinely asynchronous.

- **Testing.** The domain and service are covered by fast unit tests, and the
  HTTP layer is covered by integration tests using
  `WebApplicationFactory`, which validates routing, model validation, status
  codes and serialisation against the real pipeline.

- **Frontend kept thin.** The Angular app is a single standalone component using
  signals for state, with a small typed `TodoService` isolating all HTTP calls.
  My primary frontend background is React, so I built this Angular frontend to
  match the role's stack rather than default to what I know. The mental model
  carried over well:
 
  | React                        | Angular equivalent used here    |
  |------------------------------|---------------------------------|
  | Function component + JSX     | Standalone component + template |
  | useState                     | Signals (signal())              |
  | fetch / axios in a hook      | TodoService using HttpClient    |
  | Conditional rendering in JSX | @if / @for control-flow blocks  |
  | Vite/CRA dev proxy           | proxy.conf.json                 |
  
  I kept the app to a single component with a small, typed service so the
  structure stays easy to follow. 
  A dev-server proxy points `/api` at the backend so there is no cross-origin
  configuration to manage during development.
