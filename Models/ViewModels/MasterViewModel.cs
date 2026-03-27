namespace EMS.Models.ViewModels
{
    public class MasterViewModel
    {
        public int? TeamId { get; set; }
        public string TeamName { get; set; }

        public List<int> SelectedMembers { get; set; }
        public List<int> SelectedProjects { get; set; }
        public List<Team> Teams { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Project> Projects { get; set; }
        public List<Department> Departments { get; set; }
        public List<Role> Roles { get; set; }

        public List<TeamListVM> TeamList { get; set; }
    }
}
