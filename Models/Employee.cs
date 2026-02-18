using System;
using System.Collections.Generic;

namespace EMS.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string AadhaarNumber { get; set; } = null!;

    public string PanNumber { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? PhotoPath { get; set; }

    public int DepartmentId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
