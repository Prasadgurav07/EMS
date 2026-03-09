using System;
using System.Collections.Generic;

namespace EMS.Models;

public partial class LeaveRequest
{
    public int Id { get; set; }

    public int? Empid { get; set; }

    public DateTime? Fromdate { get; set; }

    public DateTime? Todate { get; set; }

    public string? Reason { get; set; }

    public bool? Typeofleave { get; set; }

    public bool? Status { get; set; }
}
