namespace RoyalVilla.DTO;

public class UserDTO
{
    public string Id { get; set; }
    public required string Email { get; set; } = default!;
    public required string Name { get; set; } = default!;
    public required string Role { get; set; } = default!;
}
