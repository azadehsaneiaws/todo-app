using TodoApp.Api.Models;

namespace TodoApp.Api.Repositories;

/// <summary>
/// Persistence abstraction for todos. The rest of the app depends on this
/// interface, so swapping the in-memory store for EF Core + SQL Server later
/// is a one-class change with no impact on the service or controller.
/// </summary>
public interface ITodoRepository
{
    IReadOnlyCollection<TodoItem> GetAll();

    TodoItem? GetById(Guid id);

    void Add(TodoItem item);

    /// <returns><c>true</c> if an item with a matching id was updated.</returns>
    bool Update(TodoItem item);

    /// <returns><c>true</c> if an item with a matching id was removed.</returns>
    bool Remove(Guid id);
}
