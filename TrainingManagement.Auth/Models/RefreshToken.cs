using System;

namespace TrainingManagement.Auth.Models;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Token { get; set; } = default!;

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    public string UserId { get; set; } = default!;


    public AppUser? User { get; set; }
}
