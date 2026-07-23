using TodoApp.Api.Contracts;

namespace TodoApp.Api.Services;

/// <summary>
/// Application service that holds the todo use cases. Controllers stay thin and
/// delegate here; this is also the natural seam for unit testing the behaviour
/// without spinning up the web host.
/// </summary>
public interface ITodoService
{
    IReadOnlyCollection<TodoResponse> GetAll();

    TodoResponse? GetById(Guid id);

    TodoResponse Create(CreateTodoRequest request);

    /// <returns>The updated todo, or <c>null</c> if no todo with that id exists.</returns>
    TodoResponse? Update(Guid id, UpdateTodoRequest request);

    /// <returns><c>true</c> if a todo was deleted; <c>false</c> if it did not exist.</returns>
    bool Delete(Guid id);
}
