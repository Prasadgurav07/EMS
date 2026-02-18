namespace EMS.Models.ViewModels
{
    public class UsersPageVM
    {
        public List<UserListVM> Users { get; set; } = new List<UserListVM>();

        public NewUserVM NewUser { get; set; } = new NewUserVM();
    }

}
