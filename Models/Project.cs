using System;
using System.Collections.Generic;

namespace EMS.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? ProjectDescription { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<ProjTask> ProjTasks { get; set; } = new List<ProjTask>();

    public virtual ICollection<TeamProject> TeamProjects { get; set; } = new List<TeamProject>();
}
