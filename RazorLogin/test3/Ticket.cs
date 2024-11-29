using System;
using System.Collections.Generic;

namespace RazorLogin.test3;

public partial class Ticket
{
    public int TicketId { get; set; }

    public string TicketType { get; set; } = null!;

    public int TicketPrice { get; set; }

    public DateOnly? TicketPurchaseDate { get; set; }

    public TimeOnly? TicketEntryTime { get; set; }

    public int PurchaseId { get; set; }

    public virtual Purchase Purchase { get; set; } = null!;
}
