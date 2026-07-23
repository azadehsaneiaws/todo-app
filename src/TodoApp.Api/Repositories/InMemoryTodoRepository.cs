using System.Collections.Concurrent;
using TodoApp.Api.Models;

namespace TodoApp.Api.Repositories;

/// <summary>
/// Thread-safe in-memory store. Registered as a singleton so data survives for
/// the lifetime of the process (the brief explicitly allows in-memory data).
/// A <see cref="ConcurrentDictionary{TKey,TValue}"/> keeps concurrent requests
/// from corrupting the backing collection.
/// </summary>
public sealed class InMemoryTodoRepository : ITodoRepository
{
    private readonly ConcurrentDictionary<Guid, TodoItem> _items = new();

    public IReadOnlyCollection<TodoItem> GetAll() =>
        _items.Values.OrderBy(item => item.CreatedAt).ToArray();

    public TodoItem? GetById(Guid id) =>
        _items.TryGetValue(id, out var item) ? item : null;

    public void Add(TodoItem item) =>
        _items[item.Id] = item;

    public bool Update(TodoItem item)
    {
        if (!_items.ContainsKey(item.Id))
        {
            return false;
        }

        _items[item.Id] = item;
        return true;
    }

    public bool Remove(Guid id) =>
        _items.TryRemove(id, out _);
}
