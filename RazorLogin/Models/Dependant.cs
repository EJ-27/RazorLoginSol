using System;
using System.Collections.Generic;

namespace RazorLogin.Models;



#nullable disable //evan


public partial class Dependant
{
    public int DepndantId { get; set; }

    public int EmployeeId { get; set; }

    public string? DependentName { get; set; }

    public string? DependentSex { get; set; }

    public DateOnly? DependentDob { get; set; }

    public string? Relationship { get; set; }

    public string? HealthcareTier { get; set; }
    public bool? IsDeleted { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
