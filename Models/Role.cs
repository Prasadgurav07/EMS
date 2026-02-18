using System;
using System.Collections.Generic;

namespace EMS.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<RoleFeatureAccess> RoleFeatureAccesses { get; set; } = new List<RoleFeatureAccess>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
