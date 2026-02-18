using EMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var featuresJson = HttpContext.Session.GetString("Features"); List<string> features = new List<string>(); if (featuresJson != null) { features = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(featuresJson); }
            ViewBag.Features = new SelectList(features, "FeatureName", "Featurelink");
            return View();
        }
    }
}
