using EMS.Models;
using EMS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace EMS.Controllers
{
    public class UserController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly DbHelper _dbHelper;
        private readonly EmsDbContext _db;

        public UserController(IWebHostEnvironment env, DbHelper dbHelper, EmsDbContext db)
        {
            _env = env;
            _dbHelper = dbHelper;
            _db = db;
        }
        public IActionResult Users()
        {

            var featuresJson = HttpContext.Session.GetString("Features"); List<string> features = new List<string>(); if (featuresJson != null) { features = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(featuresJson); }
            ViewBag.Features = new SelectList(features, "FeatureName", "Featurelink");
            UsersPageVM vm = new UsersPageVM();

            // fill users table
            vm.Users = _dbHelper.GetUsers();

            // fill dropdowns
            ViewBag.emps = _dbHelper.Employees();
            ViewBag.Roles = _dbHelper.GetRoles();

            return View(vm);   // ⭐ send full page model
        }
        [HttpPost]
        public IActionResult Add(UsersPageVM model)
        {
            if (!ModelState.IsValid)
            {
                // reload dropdowns if validation fails
                ViewBag.Employees = _dbHelper.GetEmployees();
                ViewBag.Roles = _dbHelper.GetRoles();
                model.Users = _dbHelper.GetUsers();
                return View("Users", model);
            }

            _dbHelper.AddUser(model.NewUser);

            TempData["msg"] = "User added successfully!";
            return RedirectToAction("Users");
        }

        public IActionResult Edit(int id)
        {

            var json = HttpContext.Session.GetString("Features");

            var features = JsonConvert.DeserializeObject<List<string>>(json);
            ViewBag.Features = new { Items = features };


            UsersPageVM vm = new UsersPageVM();

            vm.Users = _dbHelper.GetUsers();
            vm.NewUser = _dbHelper.GetUserById(id);

          
                IEnumerable<Employee> employees = _dbHelper.GetEmployees();
            ViewBag.emps = employees;
          ViewBag.Roles = _dbHelper.GetRoles();

            ViewBag.EditId = id;   // important
            return View("Users", vm);
        }
        [HttpPost]
        public IActionResult Update(UsersPageVM model, int id)
        {
            _dbHelper.UpdateUser(model.NewUser, id);
            return RedirectToAction("Users");
        }
        public IActionResult Delete(int id)
        {
            _dbHelper.DeleteUser(id);
            return RedirectToAction("Users");
        }


    }
}
