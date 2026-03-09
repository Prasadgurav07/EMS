using EMS.Models;
using EMS.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Data;
using System.Security.Claims;
using SqlCommand = Microsoft.Data.SqlClient.SqlCommand;


public class AccountController : Controller
{
    private readonly DbHelper _db;
    private readonly EmsDbContext emsDbContext;

   

    public AccountController(DbHelper db, EmsDbContext emsDbContext)
    {
        this._db = db;
        this.emsDbContext = emsDbContext;
    }

    public IActionResult Login()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        SqlCommand cmd = new SqlCommand(@"
        SELECT U.UserId,E.employeeId, U.Username, U.PasswordHash, R.RoleName,
               f.featureName, f.featurelink,
               e.FirstName, e.LastName, e.PhotoPath
        FROM Users U
        INNER JOIN Roles R ON U.RoleId = R.RoleId
        LEFT JOIN RoleFeatureAccess FA ON U.RoleId = FA.RoleId
        LEFT JOIN Features F ON FA.FeatureId = F.FeatureId
        LEFT JOIN Employees E ON U.EmployeeId = E.EmployeeId
        WHERE U.Username = @Username 
              AND U.PasswordHash = @password 
              AND U.IsActive = 1
    ");

        cmd.Parameters.Add("@Username", SqlDbType.VarChar, 100).Value = model.Username;
        cmd.Parameters.Add("@password", SqlDbType.VarChar, 100).Value = model.Password;

        DataTable dt = _db.GetDataTable(cmd);

        if (dt.Rows.Count == 0)
        {
            ViewBag.Error = "Invalid username or password";
            return View(model);
        }

        // ✅ DEFINE ROW
        var row = dt.Rows[0];

        int userId = Convert.ToInt32(row["UserId"]);
        int empid = Convert.ToInt32(row["EmployeeId"]);
        string role = Convert.ToString(row["RoleName"])?.Trim();
        string firstName = Convert.ToString(row["FirstName"]);
        string lastName = Convert.ToString(row["LastName"]);
        string photo = Convert.ToString(row["PhotoPath"]);

        // Store features in session
        List<string> features = new List<string>();

        foreach (DataRow r in dt.Rows)
        {
            if (r["featureName"] != DBNull.Value)
                features.Add(r["featureName"].ToString());

            if (r["featurelink"] != DBNull.Value)
                features.Add(r["featurelink"].ToString());
        }

        string json = JsonConvert.SerializeObject(features);
        HttpContext.Session.SetString("Features", json);

        TempData["name"] = firstName;
        // 🔥 CREATE CLAIMS
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, firstName),
        new Claim(ClaimTypes.Role, role),
        new Claim("UserId", userId.ToString()),  new Claim("EmployeeId", empid.ToString())
    };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // Redirect based on role
        if (role == "Admin")
            return RedirectToAction("dashboard",new {am="ar"});

        if (role == "Employee")
            return RedirectToAction("Dashboard");

        return RedirectToAction("Dashboard");
    }


    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
    //public IActionResult Dashboard(string? am)
    //{

    //    DashboardViewModel model=new DashboardViewModel();
    //    ViewBag.name = TempData["name"];
    //    var json = HttpContext.Session.GetString("Features");

    //    if (!string.IsNullOrEmpty(json))
    //    {
    //        var features = JsonConvert.DeserializeObject<List<string>>(json);
    //        ViewBag.Features = new { Items = features };
    //    }
    //    ViewBag.IsAdmin = User.IsInRole("Admin");
    //    var employees = _db.GetEmployeesWithDepartment();
    //    if (employees != null)
    //    {
    //        model = new DashboardViewModel
    //        {
    //            Employees = employees,
    //            NewEmployee = new Employee(),
    //            empAttendanceViewModel = new EmpAttendanceViewModel
    //            {
    //                employees = emsDbContext.Employees.ToList()

    //            }

    //        };
    //        if (model != null)
    //        {

    //            ViewBag.am = am;
    //            model.PresentToday =
    //   emsDbContext.DailyPresenties
    //   .Count(x => x.RequestDate == today && x.Status == true);

    //            model.PendingRequests =
    //                emsDbContext.DailyPresenties
    //                .Count(x => x.Status == false);

    //            model.ApprovedRequests =
    //                emsDbContext.DailyPresenties
    //                .Count(x => x.Status == true);

    //            model.RejectedRequests =
    //                emsDbContext.DailyPresenties
    //                .Count(x => x.IsRejected == true);
    //            model.empAttendanceViewModel.present_list();

    //        }

    //    }

    //    return View(model);


    //}
    public IActionResult Dashboard(string? am)
    {
        ViewBag.name = TempData["name"];

        var json = HttpContext.Session.GetString("Features");

        if (!string.IsNullOrEmpty(json))
        {
            var features = JsonConvert.DeserializeObject<List<string>>(json);
            ViewBag.Features = new { Items = features };
        }

        ViewBag.IsAdmin = User.IsInRole("Admin");
        ViewBag.am = am ?? "ar";

        var today = DateOnly.FromDateTime(DateTime.Now);

        var employees = _db.GetEmployeesWithDepartment();

        var model = new DashboardViewModel
        {
            Employees = employees ?? new List<Employee>(),
            NewEmployee = new Employee(),

            empAttendanceViewModel = new EmpAttendanceViewModel
            {
                employees = emsDbContext.Employees.ToList(),

                dailyPresenties = emsDbContext.DailyPresenties
                                    .Where(x => x.Status == false)
                                    .ToList(),

                                     leaveRequests = emsDbContext.LeaveRequests.ToList()
            }
        };

        // ✅ Dashboard counters (CORRECT PLACE)
        model.PresentToday =
            emsDbContext.DailyPresenties
            .Count(x => x.Date == today && x.Status == true);

        model.PendingRequests =
            emsDbContext.DailyPresenties
            .Count(x => x.Status == false);

        model.ApprovedRequests =
            emsDbContext.DailyPresenties
            .Count(x => x.Status == true);

        model.LeaveRequest =
           emsDbContext.LeaveRequests
           .Count(x => x.Status == false);


        // build pending list
        model.empAttendanceViewModel.present_list();

        model.empAttendanceViewModel.leaveRequest();

        return View(model);
    }



}
