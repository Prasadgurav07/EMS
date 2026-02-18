using System;
using System.Collections.Generic;

namespace EMS.Models;

public partial class RoleFeatureAccess
{
    public int RoleFeatureId { get; set; }

    public int RoleId { get; set; }

    public int FeatureId { get; set; }

    public virtual Feature Feature { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
