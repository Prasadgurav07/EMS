namespace EMS.Models.ViewModels
{
    public class NewUserVM
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; }
        public string password { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }

}
