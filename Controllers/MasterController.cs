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

    public IActionResult Master(int? deptId, int? roleId, int? teamid)
    {
        ViewBag.name = TempData["name"];

        var featuresJson = HttpContext.Session.GetString("Features");
        List<string> features = new List<string>();

        if (featuresJson != null)
        {
            features = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(featuresJson);
        }

        ViewBag.Features = new SelectList(features, "FeatureName", "Featurelink");

        var vm = new MasterViewModel
        {
            Departments = _db.Departments.ToList(),
            Roles = _db.Roles.ToList(),
            Employees = _db.Employees.ToList(),
            Teams = _db.Teams.ToList(),
            Projects = _db.Projects.ToList()
        };

        if (deptId.HasValue)
            ViewBag.EditDepartment = _db.Departments.Find(deptId.Value);

        if (roleId.HasValue)
            ViewBag.EditRole = _db.Roles.Find(roleId.Value);

        if (teamid.HasValue)
        {
            var team = _db.Teams.Find(teamid.Value);
            ViewBag.EditTeam = team;

            vm.TeamId = team.TeamId;
            vm.TeamName = team.TeamName;

            // Pre-select employees
            vm.SelectedMembers = _db.TeamMembers
                .Where(x => x.TeamId == teamid)
                .Where(x => x.EmpId.HasValue)
                .Select(x => x.EmpId.Value)
                .ToList();

            // Pre-select projects
            vm.SelectedProjects = _db.TeamProjects
                .Where(x => x.TeamId == teamid)
                .Where(x => x.ProjectId.HasValue)
                .Select(x => x.ProjectId.Value)
                .ToList();

           
        }
        vm.TeamList = _db.Teams
   .Select(t => new TeamListVM
   {
       TeamId = t.TeamId,
       TeamName = t.TeamName,

       ProjectNames = string.Join(", ",
           _db.TeamProjects
               .Where(tp => tp.TeamId == t.TeamId)
               .Join(_db.Projects,
                     tp => tp.ProjectId,
                     p => p.ProjectId,
                     (tp, p) => p.ProjectName)
       )
   })
   .ToList();
        // ✅ MUST HAVE
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
    [HttpPost]
    public IActionResult SaveTeam(MasterViewModel model)
    {
        using (var transaction = _db.Database.BeginTransaction())
        {
            try
            {
                Team team;

                if (model.TeamId.HasValue)
                {
                    // UPDATE
                    team = _db.Teams.Find(model.TeamId.Value);
                    team.TeamName = model.TeamName;

                    // Remove old mappings
                    var oldMembers = _db.TeamMembers.Where(x => x.TeamId == team.TeamId);
                    _db.TeamMembers.RemoveRange(oldMembers);

                    var oldProjects = _db.TeamProjects.Where(x => x.TeamId == team.TeamId);
                    _db.TeamProjects.RemoveRange(oldProjects);
                }
                else
                {
                    // CREATE
                    team = new Team
                    {
                        TeamName = model.TeamName
                    };

                    _db.Teams.Add(team);
                    _db.SaveChanges(); // to get TeamId
                }

                // Insert Members
                if (model.SelectedMembers != null)
                {
                    foreach (var empId in model.SelectedMembers)
                    {
                        _db.TeamMembers.Add(new TeamMember
                        {
                            TeamId = team.TeamId,
                            EmpId = empId
                        });
                    }
                }

                // Insert Projects
                if (model.SelectedProjects != null)
                {
                    foreach (var projId in model.SelectedProjects)
                    {
                        _db.TeamProjects.Add(new TeamProject
                        {
                            TeamId = team.TeamId,
                            ProjectId = projId
                        });
                    }
                }

                _db.SaveChanges();
                transaction.Commit();

                return RedirectToAction("Master");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}