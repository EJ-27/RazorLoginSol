namespace RazorLogin.ViewModels
{
    public class EnclosureViewModel
    {
        public int EnclosureId { get; set; }
        public string EnclosureName { get; set; }
        public string EnclosureDepartment { get; set; }
        public string OccupancyStatus { get; set; }
        public TimeOnly EnclosureOpenTime { get; set; }  // Changed to TimeOnly
        public TimeOnly EnclosureCloseTime { get; set; } // Changed to TimeOnly
        public TimeOnly? EnclosureCleaningTime { get; set; } // Changed to TimeOnly
        public TimeOnly? EnclosureFeedingTime { get; set; }  // Changed to TimeOnly
        public bool IsClosed { get; set; }
        public int? ZookeeperId { get; set; } // Nullable if unassigned
        public string ZookeeperName { get; set; }  // New property for the Zookeeper's name

    }


}


