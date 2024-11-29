using System;
using System.Collections.Generic;

namespace RazorLogin.test3;

public partial class Closing
{
    public int ClosingId { get; set; }

    public int EnclosureId { get; set; }

    public DateTime ClosingsStart { get; set; }

    public DateTime? ClosingsEnd { get; set; }

    public string? ClosingsReason { get; set; }

    public virtual Enclosure Enclosure { get; set; } = null!;
}
