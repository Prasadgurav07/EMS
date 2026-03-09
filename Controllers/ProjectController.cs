using EMS.Models;
using EMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EMS.Controllers
{
    public class ProjectController : Controller
    {
        private readonly EmsDbContext _context;
        private readonly DbHelper _dbHelper;
        private readonly IWebHostEnvironment _env;
        public ProjectController(EmsDbContext context,DbHelper dbHelper, IWebHostEnvironment env)
        {
            _context = context;
            _dbHelper = dbHelper;
            _env = env;
        }

        public IActionResult Index(int? id)
        {
            var featuresJson = HttpContext.Session.GetString("Features"); List<string> features = new List<string>(); if (featuresJson != null) { features = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(featuresJson); }
            ViewBag.Features = new SelectList(features, "FeatureName", "Featurelink");
  
            var project = _context.Projects.ToList();


            Project pro = new Project();

            if (id.HasValue)
            {
                pro = _context.Projects.Find(id.Value);


                if (pro == null)
                {
                    return NotFound();
                }
            }
            var model = new ProjectViewModel
            {
                projects = project,
                NewProject = pro
            };

            return View(model);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public IActionResult Index(ProjectViewModel model)
        {
            var project = model.NewProject;

           
            // =========================================================
            // STEP 3 : Call Stored Procedure (Insert / Update)
            // =========================================================
            using (SqlConnection con = new SqlConnection(_dbHelper.ConnectionString))
            {
                SqlCommand cmd;

                if (project.ProjectId == 0)
                {
                    //INSERT
                   cmd = new SqlCommand("Sp_InsertProject", con);
                }
                else
                {
                    // UPDATE
                    cmd = new SqlCommand("Sp_UpdateProject", con);
                    cmd.Parameters.AddWithValue("@projectid", project.ProjectId);
                }

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@projectname", (object?)project.ProjectName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ProjectDes", (object?)project.ProjectDescription ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StartDate", project.StartDate.HasValue
                        ? project.StartDate.Value.ToDateTime(TimeOnly.MinValue)
                        : DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", project.EndDate.HasValue
                        ? project.EndDate.Value.ToDateTime(TimeOnly.MinValue)
                        : DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", (object?)project.Status ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedDate",
      project.CreatedDate.HasValue
          ? project.CreatedDate.Value
          : (object)DBNull.Value);



                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["msg"] = project.ProjectId == 0
            ? "Employee Added Successfully"
            : "Employee Updated Successfully";

            // 🔥 clear form
            ModelState.Clear();


            return RedirectToAction("Index");

        }
    }

}
