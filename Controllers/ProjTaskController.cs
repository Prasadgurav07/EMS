using EMS.Models;
using EMS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EMS.Controllers
{
    public class ProjTaskController : Controller
    {
        private readonly EmsDbContext _db;

        public ProjTaskController(EmsDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.name = TempData["name"];
            var featuresJson = HttpContext.Session.GetString("Features"); List<string> features = new List<string>(); if (featuresJson != null) { features = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(featuresJson); }
            ViewBag.Features = new SelectList(features, "FeatureName", "Featurelink");
            int empId = Convert.ToInt32(User.FindFirst("EmployeeId")?.Value);
            string role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            //string role = User.FindFirst("Role")?.Value ?? "";

            var query = _db.ProjTasks
                .Include(t => t.Project)
                .Include(t => t.Team)
                .Include(t => t.AssignedToNavigation)
                .AsQueryable();

            // 🔥 ROLE FILTER
            if (role == "Jr. Developer")
            {
                query = query.Where(x => x.AssignedTo == empId);
            }
            else if (role == "Team Leader")
            {
                query = query.Where(x => x.CreatedBy == empId);
            }
            
                // Admin / Manager → full access

                var vm = new ProjTaskVM
                {
                    Task = new ProjTask(),
                    TaskList = query.ToList(),
                    Projects = _db.Projects.ToList(),
                    Teams = _db.Teams.ToList(),
                    Employees = _db.Employees.ToList()
                };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProjTaskVM vm)
        {
            int empId = Convert.ToInt32(User.FindFirst("EmployeeId")?.Value);
            string role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (role != "Team Leader" && role != "Admin")
            {
                return Unauthorized();
            }

            vm.Task.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            vm.Task.Status = "Pending";
            vm.Task.ApvlStatus = 1; // Pending Approval
            vm.Task.CreatedBy = empId;

            _db.ProjTasks.Add(vm.Task);
            _db.SaveChanges();

            // ✅ Get Assigned Employee Email
            var employee = _db.Employees
      .FirstOrDefault(x => x.EmployeeId == vm.Task.AssignedTo);

            var project = _db.Projects
                .FirstOrDefault(x => x.ProjectId == vm.Task.ProjectId);

            var team = _db.Teams
                .FirstOrDefault(x => x.TeamId == vm.Task.TeamId);

            if (employee != null && project != null && team != null)
            {
                await SendEmailAsync(
                    employee.Email,
                    employee.FirstName,
                    project.ProjectName,
                    vm.Task.TaskName,
                    team.TeamName,
                    vm.Task.Priority,
                    vm.Task.Status
                );
            }
            else
            {
                // Debug check (optional)
                if (employee == null) Console.WriteLine("Employee NULL");
                if (project == null) Console.WriteLine("Project NULL");
                if (team == null) Console.WriteLine("Team NULL");
            }

            TempData["msg"] = "Task Created & Sent for Approval";
            return RedirectToAction("Index");
        }
        private async Task SendEmailAsync(string toEmail, string name, string project,
                                   string taskName, string team, string priority, string status)

        {
            var fromEmail = "prasadgurav2612@gmail.com";
            var password = "lpnw ngwl czdl ailv";

            var message = new MailMessage();
            message.From = new MailAddress(fromEmail);
            message.To.Add(toEmail);
            message.Subject = "📌 New Task Assigned";

            message.IsBodyHtml = true; // ✅ IMPORTANT

            // 🎯 Priority Color
            string priorityColor = priority == "High" ? "#dc3545"
                                 : priority == "Medium" ? "#fd7e14"
                                 : "#28a745";

            // 🎯 Status Color
            string statusColor = status == "Pending" ? "#ffc107"
                               : status == "Completed" ? "#28a745"
                               : "#6c757d";

            message.Body = $@"
<div style='font-family:Segoe UI, Arial; background:#eef2f7; padding:30px;'>

    <div style='max-width:650px; margin:auto; background:#ffffff; border-radius:12px; overflow:hidden;
                box-shadow:0 5px 20px rgba(0,0,0,0.08);'>

        <!-- Header -->
        <div style='background:linear-gradient(135deg,#4e73df,#1cc88a); color:white; padding:20px;'>
            <h2 style='margin:0;'>🚀 New Task Assigned</h2>
            <p style='margin:5px 0 0;'>You’ve got a new responsibility!</p>
        </div>

        <!-- Body -->
        <div style='padding:25px; color:#444;'>

            <p>Hello <b>{name}</b>,</p>

            <p>A new task has been assigned to you. Here are the details:</p>

            <!-- Task Highlight -->
            <div style='background:#f8f9fc; padding:15px; border-left:4px solid #4e73df; border-radius:6px;'>
                <b>📌 {taskName}</b><br/>
                <small>Project: {project} | Team: {team}</small>
            </div>

            <br/>

            <!-- Status + Priority -->
            <div style='display: flex;
justify-content: space-evenly;'>

                <div style='flex:1; background:#fff3cd; padding:10px; border-radius:6px; text-align:center;'>
                    <b>Status</b><br/>
                    <span>{status}</span>
                </div>

                <div style='flex:1; background:
                    {(priority == "High" ? "#f8d7da" :
                       priority == "Medium" ? "#ffe5b4" : "#d4edda")};
                    padding:10px; border-radius:6px; text-align:center;'>
                    
                    <b>Priority</b><br/>
                    <span style='font-weight:bold;'>{priority}</span>
                </div>

            </div>

            <br/>

            <!-- Action Button -->
            <div style='text-align:center; margin-top:20px;'>
                <a href='http://yourappurl.com'
                   style='background:#1cc88a; color:white; padding:12px 25px;
                          text-decoration:none; border-radius:25px; font-weight:bold;
                          display:inline-block;'>
                    👉 View Task
                </a>
            </div>

            <br/><br/>

            <p style='font-size:13px; color:#888;'>
                This is an automated notification from EMS system.
            </p>

        </div>

    </div>

</div>";

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            smtp.Send(message);
        }

        public IActionResult Approve(int id, int status)
        {

            string role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (role != "Admin" && role != "Manager")
            {
                return Unauthorized();
            }

            var task = _db.ProjTasks.Find(id);

            if (task != null)
            {
                task.ApvlStatus = status; // 2=Approved, 3=Rejected
                _db.SaveChanges();
                TempData["msg"] = "Task Approved";
            }

            return RedirectToAction("Dashboard", "Account");
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            int empId = Convert.ToInt32(User.FindFirst("EmployeeId")?.Value);

            var task = _db.ProjTasks.Find(id);

            if (task != null && task.AssignedTo == empId)
            {
                task.Status = status; // Completed / In Progress
                _db.SaveChanges();
                TempData["app"] = "Task Updated";
                
            }
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {

            ViewBag.name = TempData["name"];
            var featuresJson = HttpContext.Session.GetString("Features"); List<string> features = new List<string>(); if (featuresJson != null) { features = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(featuresJson); }
            ViewBag.Features = new SelectList(features, "FeatureName", "Featurelink");

            var task = _db.ProjTasks
                .Include(t => t.Project)
                .Include(t => t.Team)
                .Include(t => t.AssignedToNavigation)
                .FirstOrDefault(t => t.TaskId == id);

            if (task == null)
                return NotFound();

            return View(task);
        }
    }
}
