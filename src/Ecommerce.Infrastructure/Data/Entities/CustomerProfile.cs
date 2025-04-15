using System;
using System.Collections.Generic;

namespace Ecommerce.Infrastructure.Data.Entities;

public partial class CustomerProfile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
