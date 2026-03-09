using EMS.Models;
using EMS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace EMS.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly DbHelper _dbHelper;
        private readonly EmsDbContext _db;

        public AttendanceController(IWebHostEnvironment env, DbHelper dbHelper, EmsDbContext db)
        {
            _env = env;
            _dbHelper = dbHelper;
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.name = TempData["name"];
            var featuresJson = HttpContext.Session.GetString("Features"); List<string> features = new List<string>(); if (featuresJson != null) { features = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(featuresJson); }
            ViewBag.Features = new SelectList(features, "FeatureName", "Featurelink");
            var today = DateOnly.FromDateTime(DateTime.Today);
            int empid = Convert.ToInt32(User.FindFirst("EmployeeId").Value);

            //DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            DateOnly start = new DateOnly(today.Year, today.Month, 1);
            DateOnly end = DateOnly.FromDateTime(DateTime.Today);

            // total days till today (or use DateTime.DaysInMonth for full month)
            int totalDays = today.Day;


            var vm = new EmpAttendanceViewModel
            {
                employees = _db.Employees.ToList(),
                dailyPresenties = _db.DailyPresenties.ToList(),
                
            };
            vm.dailyPresenty = (from x in vm.dailyPresenties where x.Date == today select x).FirstOrDefault();
            vm.LeaveRequest = _db.LeaveRequests
           .Count(x => x.Status == false);

            vm.LeaveApproved = _db.LeaveRequests
          .Count(x => x.Status == true && x.Empid == empid);

            //int empId = Convert.ToInt32(User.FindFirst("UserId").Value);



            // present days
            int presentDays = _db.DailyPresenties
                .Count(x =>
                    x.Empid == empid &&
                    x.Status == true &&
                    x.Date >= start &&
                    x.Date <= today);


            //    // approved leave days
            int leaveDays = _db.LeaveRequests
        .Where(x => x.Empid == empid && x.Status == true && x.Fromdate.HasValue)
        .Count(x =>
            DateOnly.FromDateTime(x.Fromdate.Value) >= start &&
            DateOnly.FromDateTime(x.Todate.Value) <= today);


            // ✅ absent
            vm.absent = totalDays - (presentDays + leaveDays);

            vm.present = presentDays;


            vm.dailyPresentyCount = (from x in vm.dailyPresenties where x.Empid == empid && x.Status == false select x).Count();
            return View(vm);

        }
        [HttpPost]
        public IActionResult CreateRaiseRequest() 
        {
            int empId = Convert.ToInt32(User.FindFirst("EmployeeId")?.Value);

            DateOnly today = DateOnly.FromDateTime(DateTime.Now); 

            var existreq = _db.DailyPresenties.Where(x => x.Empid == Convert.ToInt32(User.FindFirst("EmployeeId").Value) && x.Date == DateOnly.FromDateTime(DateTime.Now)).FirstOrDefault();

            if (existreq!=null)
            {
                TempData["msgtext"] = "Daily Presenty Allready Marked";
                return RedirectToAction("Index");
            }



            // ✅ Check Leave Request exists for today
            var existLeave = _db.LeaveRequests
                .FirstOrDefault(x =>
                    x.Empid == empId &&
                    x.Fromdate.HasValue &&
                    x.Todate.HasValue &&
                    DateOnly.FromDateTime(x.Fromdate.Value) <= today &&
                    DateOnly.FromDateTime(x.Todate.Value) >= today &&
                    x.Status == true   // approved leave
                );

            if (existLeave != null)
            {
                TempData["msgtext"] = "You already have leave for today";
                return RedirectToAction("Index");
            }


            var req = _db.DailyPresenties.Add(
                    new DailyPresenty()
                    {

                        Empid = Convert.ToInt32(User.FindFirst("EmployeeId")?.Value),
                        PunchIn = DateTime.Now,
                        PunchOut =DateTime.Now.AddHours(8) ,
                        Date = DateOnly.FromDateTime(DateTime.Now),
                    }
                    );
           
            _db.SaveChanges();
            TempData["msg"] = "Daily Presenty Marked Successfully";
            return RedirectToAction("Index"); 
           
        }
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var req = _db.DailyPresenties.Find(id);

            if (req != null)
            {
                req.Status = true;
                _db.SaveChanges();
            }

            TempData["app"] = "Request Approved";
            return RedirectToAction("Dashboard", "Account");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var req = _db.DailyPresenties.Find(id);

            if (req != null)
            {
                req.Status =false;
                _db.SaveChanges();
            }

            TempData["rej"] = "Request Rejected";
            return RedirectToAction("Dashboard", "Account");
        }

        [HttpPost]
        public IActionResult CreateRequest(DateOnly FromDate, DateOnly ToDate, string Reason ,int EmployeeId)
        {
            
            if (FromDate == default || ToDate == default)
            {
                TempData["msgtext"] = "Please select valid dates";
                return RedirectToAction("Dashboard");
            }

            DateTime fromDate = FromDate.ToDateTime(TimeOnly.MinValue);
            DateTime toDate = ToDate.ToDateTime(TimeOnly.MinValue);

            bool exists = _db.LeaveRequests.Any(x =>
                x.Empid == EmployeeId &&
                fromDate <= x.Todate &&
                toDate >= x.Fromdate
            );
            // check overlap
           
            if (exists)
            {
                TempData["msgtext"] = "Leave already exists for selected dates.";
                return RedirectToAction("Index");
            }

            var leave = new LeaveRequest
            {
                Empid = EmployeeId,
                Fromdate = fromDate,
                Todate = toDate,
                Reason = Reason,
                Status = false
            };

            _db.LeaveRequests.Add(leave);
            _db.SaveChanges();

            TempData["msg"] = "Leave request submitted.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult LeaveApprove(int id)
        {
            var req = _db.LeaveRequests.Find(id);

            if (req != null)
            {
                req.Status = true;
                _db.SaveChanges();
            }

            TempData["app"] = "Request Approved";
            return RedirectToAction("Dashboard", "Account");
        }

        [HttpPost]
        public IActionResult LeaveReject(int id)
        {
            var req = _db.LeaveRequests.Find(id);

            if (req != null)
            {
                req.Status = false;
                _db.SaveChanges();
            }

            TempData["rej"] = "Request Rejected";
            return RedirectToAction("Dashboard", "Account");
        }
    }
}
