using EMS.Models;
using EMS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Admin")]
public class MasterController : Controller
{
    private readonly EmsDbContext _db;

    public MasterController(EmsDbContext db)
    {
        _db = db;
    }

    public IActionResult Master(int? deptId, int? roleId)
    {
        ViewBag.name = TempData["name"];
        var featuresJson = HttpContext.Session.GetString("Features"); List<string> features = new List<string>(); if (featuresJson != null) { features = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(featuresJson); }
        ViewBag.Features = new SelectList(features, "FeatureName", "Featurelink");
        var vm = new MasterViewModel
        {
            Departments = _db.Departments.ToList(),
            Roles = _db.Roles.ToList()
        };

        if (deptId.HasValue)
            ViewBag.EditDepartment =
                _db.Departments.Find(deptId.Value);

        if (roleId.HasValue)
            ViewBag.EditRole =
                _db.Roles.Find(roleId.Value);

        return View(vm);
    }

    [HttpPost]
    public IActionResult add(int? DepartmentId, string DepartmentName)
    {
        if (DepartmentId == null || DepartmentId == 0)
            _db.Departments.Add(new Department { DepartmentName = DepartmentName });
        else
            _db.Departments.Find(DepartmentId).DepartmentName = DepartmentName;

        _db.SaveChanges();
        return RedirectToAction("Master");
    }

    [HttpPost]
    public IActionResult SaveRole(int? RoleId, string RoleName)
    {
        if (RoleId == null || RoleId == 0)
            _db.Roles.Add(new Role { RoleName = RoleName });
        else
            _db.Roles.Find(RoleId).RoleName = RoleName;

        _db.SaveChanges();
        TempData["msg"] = "Role Added successfully!";
        return RedirectToAction("Master");
    }

    public IActionResult DeleteDepartment(int id)
    {
        var dept = _db.Departments.Find(id);
        if (dept != null)
        {
            _db.Departments.Remove(dept);
            _db.SaveChanges();
        }
        return RedirectToAction("Master");
    }

    public IActionResult DeleteRole(int id)
    {
        var role = _db.Roles.Find(id);
        if (role != null)
        {
            _db.Roles.Remove(role);
            _db.SaveChanges();
        }
        return RedirectToAction("Master");
    }

    [HttpPost]
    public IActionResult Save(RoleFeatureVM model)
    {
        // Remove old mappings
        var old = _db.RoleFeatureAccesses
            .Where(x => x.RoleId == model.SelectedRoleId);

        _db.RoleFeatureAccesses.RemoveRange(old);

        // Add selected features
        var selected = model.Features
            .Where(x => x.IsSelected)
            .Select(x => new RoleFeatureAccess
            {
                RoleId = model.SelectedRoleId,
                FeatureId = x.FeatureId
            });

        _db.RoleFeatureAccesses.AddRange(selected);

        _db.SaveChanges();
        TempData["msg"] = "Access granted successfully !";
        return RedirectToAction("Index", new { roleId = model.SelectedRoleId });
    }
    public IActionResult Index(int? roleId)
    {
        ViewBag.name = TempData["name"];
        var featuresJson = HttpContext.Session.GetString("Features"); List<string> features = new List<string>(); if (featuresJson != null) { features = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(featuresJson); }
        ViewBag.Features = new SelectList(features, "FeatureName", "Featurelink");
        var vm = new RoleFeatureVM
        {
            Roles = _db.Roles.ToList(),
            SelectedRoleId = roleId ?? 0
        };

        if (roleId.HasValue)
        {
            var assigned = _db.RoleFeatureAccesses
                .Where(x => x.RoleId == roleId)
                .Select(x => x.FeatureId)
                .ToList();

            vm.Features = _db.Features
                .Select(f => new FeatureCheckboxVM
                {
                    FeatureId = f.FeatureId,
                    FeatureName = f.FeatureName,
                    IsSelected = assigned.Contains(f.FeatureId)
                }).ToList();
        }
        else
        {
            vm.Features = new List<FeatureCheckboxVM>();
        }

        return View(vm);
    }
}