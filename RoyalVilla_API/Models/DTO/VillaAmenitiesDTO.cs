using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoyalVilla_API.Models.DTO;

public class VillaAmenitiesDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    [Required]
    public int VillaId { get; set; }
    public string? VillaName { get; set; }
}
