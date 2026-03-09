namespace EMS.Models.ViewModels
{
    public class RoleFeatureVM
    {
        public int SelectedRoleId { get; set; }
        public List<Role> Roles { get; set; }
        public List<FeatureCheckboxVM> Features { get; set; }
    }
}
