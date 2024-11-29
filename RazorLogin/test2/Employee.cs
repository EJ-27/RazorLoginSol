using System;
using System.Collections.Generic;

namespace RazorLogin.test2;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int Ssn { get; set; }

    public string? EmployeeFirstName { get; set; }

    public string? EmployeeLastName { get; set; }

    public DateOnly? EmployeeDob { get; set; }

    public string? EmployeeAddress { get; set; }

    public int? EmployeeSalary { get; set; }

    public DateOnly? DateOfEmployment { get; set; }

    public string? Degree { get; set; }

    public string? EmployeePhoneNumber { get; set; }

    public int? SupervisorId { get; set; }

    public string? Department { get; set; }

    public int? ShopId { get; set; }

    public int? FoodStoreId { get; set; }

    public string EmployeeEmail { get; set; } = null!;

    public virtual ICollection<Dependant> Dependants { get; set; } = new List<Dependant>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual FoodStore? FoodStore { get; set; }

    public virtual Manager? Manager { get; set; }

    public virtual GiftShop? Shop { get; set; }

    public virtual Manager? Supervisor { get; set; }

    public virtual Zookeeper? Zookeeper { get; set; }
}
