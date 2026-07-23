namespace TodoApp.Api.Models;

/// <summary>
/// A single task on a user's TODO list. The domain model owns its own
/// invariants (a todo always has a non-empty title) rather than trusting
/// callers to keep it in a valid state.
/// </summary>
public sealed class TodoItem
{
    public Guid Id { get; }

    public string Title { get; private set; }

    public bool IsCompleted { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    /// <summary>Creates a brand new, uncompleted todo.</summary>
    public TodoItem(string title)
        : this(Guid.NewGuid(), title, isCompleted: false, DateTimeOffset.UtcNow)
    {
    }

    /// <summary>
    /// Full constructor, used when rehydrating an existing item (e.g. from a
    /// store) or from tests that need deterministic values.
    /// </summary>
    public TodoItem(Guid id, string title, bool isCompleted, DateTimeOffset createdAt)
    {
        Id = id;
        Title = Normalise(title);
        IsCompleted = isCompleted;
        CreatedAt = createdAt;
    }

    public void Update(string title, bool isCompleted)
    {
        Title = Normalise(title);
        IsCompleted = isCompleted;
    }

    private static string Normalise(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("A todo title cannot be empty.", nameof(title));
        }

        return title.Trim();
    }
}
