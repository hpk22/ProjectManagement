namespace ProjectManagement.Models
{
    using System;
    using System.Collections.Generic;

    public class Task
    {
        public int TaskID { get; set; }
        public int ProjectID { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public double EstimatedHours { get; set; }
        public double? ActualHours { get; set; }
        public int AssignedTo { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ProjectName { get; set; }
        public string AssigneeName { get; set; }


        //public List<TaskDependency> Dependencies { get; set; } = new List<TaskDependency>();
    }

   
}
