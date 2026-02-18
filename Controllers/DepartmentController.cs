using EMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly EmsDbContext _context;

        public DepartmentController(EmsDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var list = _context.Departments.ToList();
            return View(list);
        }
        [HttpGet]
        public IActionResult Add() 
        { 
            return View();
        }
        [HttpPost]
        public IActionResult Add(Department department) 
        { 
            var add = _context.Add(department);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
