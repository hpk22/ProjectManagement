using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectTeamMember
    {
        public int UserID { get; set; }
        public string Role { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? ExitDate { get; set; }
    }
}