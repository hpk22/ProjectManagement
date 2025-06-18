using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ProjectManagement.Models
{
    public class Project
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public decimal Budget { get; set; }
        public int ClientID { get; set; }
        public int ManagerID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ManagerName { get; set; }
        public string ClientName { get; set; }
    }

}