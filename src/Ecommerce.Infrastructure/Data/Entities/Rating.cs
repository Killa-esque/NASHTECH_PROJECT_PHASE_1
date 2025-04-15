using System;
using System.Collections.Generic;

namespace Ecommerce.Infrastructure.Data.Entities;

public partial class Rating
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid UserId { get; set; }

    public int? RatingValue { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
