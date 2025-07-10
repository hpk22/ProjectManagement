using System;

namespace ProjectManagement.Models
{
    public class TimeSheet
    {
        public int TimeSheetID { get; set; }
        public int UserID { get; set; }
        public int ProjectID { get; set; }
        public int? TaskID { get; set; }
        public DateTime WorkDate { get; set; }
        public decimal HoursWorked { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // Submitted, Approved, Rejected
        public DateTime SubmittedDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string UserName { get; set; }
        public string ProjectName { get; set; }
        //public int? TaskID { get; set; }
        public string TaskName { get; set; }


    }
}
