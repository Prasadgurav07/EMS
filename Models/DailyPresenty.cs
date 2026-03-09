using System;
using System.Collections.Generic;

namespace EMS.Models;

public partial class DailyPresenty
{
    public int Id { get; set; }

    public int Empid { get; set; }

    public DateTime PunchIn { get; set; }

    public DateTime PunchOut { get; set; }

    public bool Status { get; set; }

    public DateOnly Date { get; set; }
}
