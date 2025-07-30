using connect4_backend.Data.Models;

namespace connect4_backend.Data.DTOs;

public class ProfileDto
{
    public string email { get; set; } = null!;

    public string firstName { get; set; } = null!;

    public string lastName { get; set; } = null!;

    public string? bio { get; set; }

    public string? avatarUrl { get; set; }

    public string? coverUrl { get; set; }

    public DateTime createdAt { get; set; }

    public DateTime updatedAt { get; set; }

    public static ProfileDto fromProfile(Profile profile)
    => new ProfileDto()
    {
        firstName = profile.FirstName,
        lastName = profile.LastName,
        email = profile.Email,
        bio = profile.Bio,
        avatarUrl = profile.AvatarUrl,
        coverUrl = profile.CoverUrl,
        updatedAt = profile.UpdatedAt,
        createdAt = profile.CreatedAt
    };
}