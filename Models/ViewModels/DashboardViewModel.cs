namespace EMS.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<Employee> Employees { get; set; }
        public Employee NewEmployee { get; set; }
        public List<string>? Features { get; internal set; }

        public  EmpAttendanceViewModel empAttendanceViewModel { get; set; }

        public List<Project> projects { get; set; }
        public List<Team> teams { get; set; }
        public int PresentToday { get; set; }

        public int PendingRequests { get; set; }

        public int ApprovedRequests { get; set; }
        public int TaskRequests { get; set; }
        public int totalProjects { get; set; }

        public int RejectedRequests { get; set; }
        public int LeaveRequest { get; set; }
    }
}
