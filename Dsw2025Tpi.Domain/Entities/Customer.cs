using System.ComponentModel.DataAnnotations;

namespace Dsw2025Tpi.Domain.Entities;

public class Customer : EntityBase
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

