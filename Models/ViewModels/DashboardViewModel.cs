namespace EMS.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<Employee> Employees { get; set; }
        public Employee NewEmployee { get; set; }
        public List<string>? Features { get; internal set; }
    }
}
