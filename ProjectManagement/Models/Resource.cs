using System;

namespace ProjectManagement.Models
{
    public class Resource
    {
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public string ResourceType { get; set; } // e.g., Human, Equipment
        public decimal? Cost { get; set; }        // Cost per hour or unit
        public decimal Availability { get; set; } // Percentage of availability
        public int? UserID { get; set; }          // Nullable for non-human resources
    }
}
