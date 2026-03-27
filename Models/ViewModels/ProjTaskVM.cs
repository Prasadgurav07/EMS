namespace EMS.Models.ViewModels
{
    public class ProjTaskVM
    {
        public ProjTask Task { get; set; }
        public Project project { get; set; }
        public Team team { get; set; }

        public List<ProjTask> TaskList { get; set; }

        public List<Project> Projects { get; set; }
        public List<Team> Teams { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
