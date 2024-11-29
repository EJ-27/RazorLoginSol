using System;
using System.Collections.Generic;

namespace RazorLogin.Models;

public partial class Enclosure
{
    public int EnclosureId { get; set; }

    public string? EnclosureName { get; set; }

    public string? EnclosureDepartment { get; set; }

    public TimeOnly EnclosureOpenTime { get; set; }

    public TimeOnly EnclosureCloseTime { get; set; }

    public TimeOnly EnclosureCleaningTime { get; set; }

    public TimeOnly EnclosureFeedingTime { get; set; }

    public int ZookeeperId { get; set; }

    public string? OccupancyStatus { get; set; }
    public bool? IsDeleted { get; set; }


    public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();

    public virtual ICollection<Closing> Closings { get; set; } = new List<Closing>();

    public virtual Zookeeper? Zookeeper { get; set; } = null!; //added ? 11/10 to fix edit page
}
