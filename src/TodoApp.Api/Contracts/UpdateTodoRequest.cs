using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Contracts;

/// <summary>Payload for updating an existing todo (rename and/or toggle done).</summary>
public sealed class UpdateTodoRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    public bool IsCompleted { get; init; }
}
