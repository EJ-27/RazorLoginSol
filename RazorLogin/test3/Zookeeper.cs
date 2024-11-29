using System;
using System.Collections.Generic;

namespace RazorLogin.test3;

public partial class Zookeeper
{
    public int ZookeeperId { get; set; }

    public int EmployeeId { get; set; }

    public short? NumAssignedCages { get; set; }

    public DateOnly? TrainingRenewalDate { get; set; }

    public DateOnly? LastTrainingDate { get; set; }

    public string? AssignedDepartment { get; set; }

    public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<Enclosure> Enclosures { get; set; } = new List<Enclosure>();
}
