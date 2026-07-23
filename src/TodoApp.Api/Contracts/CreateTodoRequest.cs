using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Contracts;

/// <summary>Payload for creating a new todo.</summary>
public sealed class CreateTodoRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;
}
