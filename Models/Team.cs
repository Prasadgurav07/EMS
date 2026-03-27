using System;
using System.Collections.Generic;

namespace EMS.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public string? TeamName { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<ProjTask> ProjTasks { get; set; } = new List<ProjTask>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    public virtual ICollection<TeamProject> TeamProjects { get; set; } = new List<TeamProject>();
}
