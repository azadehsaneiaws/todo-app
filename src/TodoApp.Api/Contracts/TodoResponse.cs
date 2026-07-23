using TodoApp.Api.Models;

namespace TodoApp.Api.Contracts;

/// <summary>
/// What the API returns to clients. Kept separate from <see cref="TodoItem"/>
/// so the wire contract can evolve independently of the domain model.
/// </summary>
public sealed record TodoResponse(Guid Id, string Title, bool IsCompleted, DateTimeOffset CreatedAt)
{
    public static TodoResponse FromDomain(TodoItem item) =>
        new(item.Id, item.Title, item.IsCompleted, item.CreatedAt);
}
