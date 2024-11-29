using System;
using System.Collections.Generic;

namespace RazorLogin.Models;

public partial class Purchase
{
    public int PurchaseId { get; set; }

    public int? CustomerId { get; set; }

    public int? StoreId { get; set; }

    public int? NumItems { get; set; }

    public int? TotalPurchasesPrice { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public string? PurchaseMethod { get; set; }

    public TimeOnly? PurchaseTime { get; set; }

    public string? ItemName { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
