using System;

namespace ProjectManagement.Models
{
    public class ProjectStatusReport
    {
        public string Status { get; set; }
        public int Total { get; set; }
    }

    public class ResourceUtilizationReport
    {
        public string ResourceName { get; set; }
        public decimal AllocationPercentage { get; set; }
        public DateTime AllocationStartDate { get; set; }
        public DateTime AllocationEndDate { get; set; }
    }

    public class TaskStatusReport
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }

    public class TimesheetSummaryReport
    {
        public int UserID { get; set; }
        public decimal TotalHours { get; set; }
    }
}
