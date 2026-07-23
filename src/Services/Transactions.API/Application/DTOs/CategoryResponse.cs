using System;

namespace Transactions.API.Application.DTOs;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Color { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
}
