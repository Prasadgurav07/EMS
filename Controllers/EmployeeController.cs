using EMS.Models;
using EMS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

public class EmployeeController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly DbHelper _dbHelper;
    private readonly EmsDbContext _db;

    public EmployeeController(IWebHostEnvironment env, DbHelper dbHelper,EmsDbContext db)
    {
        _env = env;
        _dbHelper = dbHelper;
        _db = db;
    }

    [HttpGet]
    public IActionResult Add(int? id)
    
    {
        ViewBag.name = TempData["name"];
        var featuresJson = HttpContext.Session.GetString("Features"); List<string> features = new List<string>(); if (featuresJson != null) { features = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(featuresJson); }
        ViewBag.Features = new SelectList(features, "FeatureName", "Featurelink");
        var employees = _dbHelper.GetEmployeesWithDepartment();
        var departments = _dbHelper.GetDepartments();

        ViewBag.Departments = new SelectList(departments, "DepartmentId", "DepartmentName");

        Employee emp = new Employee();
            
        if (id.HasValue)
        {
            emp = _dbHelper.GetEmployeeById(id.Value);


            if (emp == null)
            {
                return NotFound();
            }
        }

        var model = new DashboardViewModel
        {
            Employees = employees,
            NewEmployee = emp
        };

        return View(model);
    }



    [HttpPost]
    public IActionResult Add(DashboardViewModel model, IFormFile Photo)
    {
        var employee = model.NewEmployee;

        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        string oldPhotoPath = null;

        // =========================================================
        // STEP 1 : If updating → get existing photo from DB
        // =========================================================
        if (employee.EmployeeId != 0)
        {
            using (SqlConnection con = new SqlConnection(_dbHelper.ConnectionString))
            {
                SqlCommand getCmd = new SqlCommand(
                    "SELECT PhotoPath FROM Employees WHERE EmployeeId=@id", con);

                getCmd.Parameters.AddWithValue("@id", employee.EmployeeId);
                con.Open();

                var result = getCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    oldPhotoPath = result.ToString();
            }
        }

        // =========================================================
        // STEP 2 : Upload new photo (if user selected new photo)
        // =========================================================
        if (Photo != null && Photo.Length > 0)
        {
            // 🧹 Delete old photo if exists
            if (!string.IsNullOrEmpty(oldPhotoPath))
            {
                string oldFileFullPath = Path.Combine(
                    _env.WebRootPath, oldPhotoPath.TrimStart('/'));

                if (System.IO.File.Exists(oldFileFullPath))
                    System.IO.File.Delete(oldFileFullPath);
            }

            // 📸 Save new photo
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Photo.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Photo.CopyTo(stream);
            }

            employee.PhotoPath = "/uploads/" + fileName;
        }
        else
        {
            // 🟢 No new photo → keep old photo
            employee.PhotoPath = oldPhotoPath;
        }

        // =========================================================
        // STEP 3 : Call Stored Procedure (Insert / Update)
        // =========================================================
        using (SqlConnection con = new SqlConnection(_dbHelper.ConnectionString))
        {
            SqlCommand cmd;

            if (employee.EmployeeId == 0)
            {
                // INSERT
                cmd = new SqlCommand("Sp_InsertEmployee", con);
            }
            else
            {
                // UPDATE
                cmd = new SqlCommand("Sp_UpdateEmployee", con);
                cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
            }

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FirstName", (object?)employee.FirstName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LastName", (object?)employee.LastName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)employee.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", (object?)employee.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AadhaarNumber", (object?)employee.AadhaarNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PanNumber", (object?)employee.PanNumber ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@DateOfBirth",
                employee.DateOfBirth.HasValue
                    ? employee.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue)
                    : DBNull.Value);

            cmd.Parameters.AddWithValue("@Gender", (object?)employee.Gender ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhotoPath", (object?)employee.PhotoPath ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DepartmentId", employee.DepartmentId);
            cmd.Parameters.AddWithValue("@IsActive", 1);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        TempData["msg"] = employee.EmployeeId == 0
        ? "Employee Added Successfully"
        : "Employee Updated Successfully";

        // 🔥 clear form
        ModelState.Clear();


        return RedirectToAction("Add");

    }


}
