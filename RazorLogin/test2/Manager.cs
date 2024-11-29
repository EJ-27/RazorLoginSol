using System;
using System.Collections.Generic;

namespace RazorLogin.test2;

public partial class Manager
{
    public int ManagerId { get; set; }

    public int EmployeeId { get; set; }

    public string Department { get; set; } = null!;

    public DateOnly? ManagerEmploymentDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
