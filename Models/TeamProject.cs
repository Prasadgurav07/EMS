using System;
using System.Collections.Generic;

namespace EMS.Models;

public partial class TeamProject
{
    public int Id { get; set; }

    public int? TeamId { get; set; }

    public int? ProjectId { get; set; }

    public virtual Project? Project { get; set; }

    public virtual Team? Team { get; set; }
}
