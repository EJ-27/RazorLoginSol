using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RazorLogin.Models;

public partial class Manager
{
    public int ManagerId { get; set; }

    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Department is required.")]
    public string Department { get; set; } = null!;

    public bool? IsDeleted { get; set; }


    public DateOnly? ManagerEmploymentDate { get; set; }

    public virtual Employee? Employee { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
