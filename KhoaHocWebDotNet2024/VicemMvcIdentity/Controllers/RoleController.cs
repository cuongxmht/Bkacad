using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VicemMvcIdentity.Models.Process;
using VicemMvcIdentity.Models.ViewModels;

namespace VicemMvcIdentity.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager=roleManager;
        }
        
        // [Authorize(Policy = nameof(SystemPermissions.RoleView))]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        // [Authorize(Policy = nameof(SystemPermissions.RoleCreate))]
        public IActionResult Create() => View();
        
        // [Authorize(Policy = nameof(SystemPermissions.RoleCreate))]
        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = new IdentityRole(roleName.Trim());
                await _roleManager.CreateAsync(role);
            }
            return RedirectToAction("Index");
        }
        // [Authorize(Policy = nameof(SystemPermissions.RoleDelete))]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }
        // [Authorize(Policy = nameof(SystemPermissions.RoleDelete))]
        [HttpPost]
        public async Task<IActionResult> Edit(string id, string newName)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            role.Name = newName;
            await _roleManager.UpdateAsync(role);
            return RedirectToAction("Index");
        }
        // [Authorize(Policy = nameof(SystemPermissions.RoleDelete))]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }
        // [Authorize(Policy = nameof(SystemPermissions.AssignClaim))]
        public async Task<IActionResult> AssignClaim(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            { return BadRequest(); }
            var allPermissions = Enum.GetValues(typeof(SystemPermissions)).Cast<SystemPermissions>().Select(p => p.ToString()).ToList();
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            if (roleClaims == null)
            {
                roleClaims = new List<Claim>();
            }
            var model = new RoleClaimVM
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Claims = allPermissions.Select(p => new RoleClaim
                {
                    Type = "Permission",
                    Value = p,
                    Selected = roleClaims.Any(c => c.Type == "Permission" && c.Value == p)
                }).ToList()
            };
            return View(model);
        }
        // [Authorize(Policy = nameof(SystemPermissions.AssignClaim))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignClaim(RoleClaimVM model)
        {
            if (!ModelState.IsValid)
            { return View(model); }
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            { return BadRequest(); }
            var claims = await _roleManager.GetClaimsAsync(role);
            if (claims == null)
            {
                claims = new List<Claim>();
            }
            foreach (var claim in claims.Where(c => c.Type == "Permission"))
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
            foreach (var claim in model.Claims.Where(c => c.Selected))
            {
                await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}