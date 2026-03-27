using System;
using System.Collections.Generic;

namespace EMS.Models;

public partial class TeamMember
{
    public int Id { get; set; }

    public int? TeamId { get; set; }

    public int? EmpId { get; set; }

    public virtual Team? Team { get; set; }
}
