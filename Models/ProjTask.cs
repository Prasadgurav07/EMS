using System;
using System.Collections.Generic;

namespace EMS.Models;

public partial class ProjTask
{
    public int TaskId { get; set; }

    public string? TaskName { get; set; }

    public string? Priority { get; set; }

    public int? ProjectId { get; set; }

    public int? AssignedTo { get; set; }

    public int? TeamId { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public int? UpdBy { get; set; }

    public string? UpdDate { get; set; }

    public int? ApvlStatus { get; set; }

    public virtual Employee? AssignedToNavigation { get; set; }

    public virtual Project? Project { get; set; }

    public virtual Team? Team { get; set; }
}
