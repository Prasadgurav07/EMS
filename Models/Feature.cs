using System;
using System.Collections.Generic;

namespace EMS.Models;

public partial class Feature
{
    public int FeatureId { get; set; }

    public string FeatureName { get; set; } = null!;

    public string FeatureKey { get; set; } = null!;

    public string FeatureLink { get; set; } = null!;

    public string? AllowedRole { get; set; }

    public virtual ICollection<RoleFeatureAccess> RoleFeatureAccesses { get; set; } = new List<RoleFeatureAccess>();
}
