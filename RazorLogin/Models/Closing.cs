using System;
using System.Collections.Generic;

namespace RazorLogin.Models;

public partial class Closing
{
    public int ClosingId { get; set; }

    public DateTime ClosingsEnd { get; set; }

    public DateTime ClosingsStart { get; set; }

    public string? ClosingsReason { get; set; }

    public int EnclosureId { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Enclosure? Enclosure { get; set; } = null!; //change add ? 11/10 to allow creation and edit
}
