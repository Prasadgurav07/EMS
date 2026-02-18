using AspNetCoreGeneratedDocument;
using BCrypt.Net;
using EMS.Models;
using EMS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Data;
using System.Data.SqlClient;
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
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        SqlCommand cmd = new SqlCommand(@"
               SELECT U.UserId, U.Username, U.PasswordHash, R.RoleName,f.featureName,f.featurelink,e.FirstName,e.LastName,e.PhotoPath
       FROM Users U
       INNER JOIN Roles R ON U.RoleId = R.RoleId
left join RoleFeatureAccess FA on u.RoleId = FA.RoleId
	   
left join  Features F on fa.FeatureId = f.FeatureId
left join Employees E on u.EmployeeId = e.EmployeeId
        WHERE U.Username = @Username AND u.PasswordHash = @password and U.IsActive = 1
    ");

        cmd.Parameters.Add("@Username", SqlDbType.VarChar, 100).Value = model.Username;
        cmd.Parameters.Add("@password", SqlDbType.VarChar, 100).Value = model.Password;

        DataTable dt = _db.GetDataTable(cmd);

        if (dt.Rows.Count == 0)
        {
            ViewBag.Error = "Invalid username or password";
            return View(model);
        }

        // 🔹 Read values from DB
        int userId = Convert.ToInt32(dt.Rows[0]["UserId"]);
        string role = Convert.ToString(dt.Rows[0]["RoleName"])?.Trim();
        string firstName = Convert.ToString(dt.Rows[0]["FirstName"]);
        string lastName = Convert.ToString(dt.Rows[0]["LastName"]);
        string photo = Convert.ToString(dt.Rows[0]["PhotoPath"]);
        // ADMIN ONLY ACCESS
        
        // STORE FEATURES IN SESSION
        List<string> features = new List<string>();

        foreach (DataRow row in dt.Rows)
        {
            features.Add(row["featureName"].ToString());
            features.Add(row["featurelink"].ToString());
        }

        string json = JsonConvert.SerializeObject(features);
        HttpContext.Session.SetString("Features", json);

        // SESSION
        HttpContext.Session.SetInt32(
            "UserId", Convert.ToInt32(dt.Rows[0]["UserId"])
        );
        // 🔹 STORE SESSION DATA (VERY IMPORTANT)
        HttpContext.Session.SetInt32("UserId", userId);
        HttpContext.Session.SetString("UserName", firstName + " " + lastName);
        HttpContext.Session.SetString("Photo", photo ?? "");
        HttpContext.Session.SetString("Role", role);

        TempData["name"] = firstName;
        ViewBag.photo = photo;
        ViewBag.featues = dt;

        if (role == "Admin")
        {
            return RedirectToAction("Add", "Employee");
        }
        else if (role == "Employee")
        {
            return RedirectToAction("EmpDashborad");
        }
        else 
        { 

        }
        return BadRequest("You Have not Access");
    }


    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
    public IActionResult Dashboard()
    {
        var json = HttpContext.Session.GetString("Features");

        if (!string.IsNullOrEmpty(json))
        {
            var features = JsonConvert.DeserializeObject<List<string>>(json);
            ViewBag.Features = new { Items = features };
        }
        var employees = _db.GetEmployeesWithDepartment();
      

       

        var model = new DashboardViewModel
        {
            Employees = employees,
            NewEmployee = new Employee(),

        };

        return View(model);
    }

    public IActionResult EmpDashborad() 
    {
        var featuresJson = HttpContext.Session.GetString("Features"); List<string> features = new List<string>(); if (featuresJson != null) { features = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(featuresJson); }
        ViewBag.Features = new SelectList(features, "FeatureName", "Featurelink");
        return View();
    }



}
