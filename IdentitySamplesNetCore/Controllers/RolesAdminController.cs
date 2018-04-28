using IdentitySamplesNetCore.Models;
using IdentitySamplesNetCore.Models.RolesAdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IdentitySamplesNetCore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesAdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesAdminController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.OrderBy(x=>x.Name);
            return View(roles);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var role = await _roleManager.FindByIdAsync(id);
            var users = new List<ApplicationUser>();

            foreach (var user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    users.Add(user);
                }
            }
            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return View(role);

        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.Name);
                var roleResult = await _roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError("", roleResult.Errors.First().Description);
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            RoleViewModel roleViewModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            };
            return View(roleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind(nameof(RoleViewModel.Name), nameof(RoleViewModel.Id))]RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(roleViewModel.Id);
                role.Name = roleViewModel.Name;
                await _roleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return BadRequest();
                }
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }
                IdentityResult result;
                if (deleteUser != null)
                {
                    result = await _roleManager.DeleteAsync(role);
                }
                else
                {
                    result = await _roleManager.DeleteAsync(role);
                }
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().Description);
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
