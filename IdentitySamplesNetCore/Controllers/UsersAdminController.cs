using IdentitySamplesNetCore.Models;
using IdentitySamplesNetCore.Models.UsersAdminViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentitySamplesNetCore.Controllers
{
    public class UsersAdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersAdminController(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.OrderBy(x => x.UserName).ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.RoleNames = await _userManager.GetRolesAsync(user);
            return View(user);
        }

        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.OrderBy(x=>x.Name).ToListAsync();
            ViewBag.RoleId = new SelectList(roles, "Name", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserRegisterViewModel viewModel, 
            params string[] selectedRoles)
        {
            var roles = await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = viewModel.Email,
                    Email = viewModel.Email,
                };
                var result = await _userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    if (selectedRoles != null && selectedRoles.Any())
                    {
                         result = await _userManager.AddToRolesAsync(user, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First().Description);
                            ViewBag.RoleId = new SelectList(roles, "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", result.Errors.First().Description);
                    ViewBag.RoleId = new SelectList(roles, "Name", "Name");
                    return View();
                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(roles, "Name", "Name");
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            return View(new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                RolesList = _roleManager.Roles.ToList()
                .Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind(nameof(EditUserViewModel.Email),nameof(EditUserViewModel.Id))] EditUserViewModel viewModel,
            params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(viewModel.Id);
                if (user == null)
                {
                    return NotFound();
                }
                user.UserName = viewModel.Email;
                user.Email = viewModel.Email;

                var userRoles = await _userManager.GetRolesAsync(user);
                selectedRole = selectedRole ?? new string[] { };

                var result = await _userManager.AddToRolesAsync(
                    user, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().Description);
                    return View();
                }

                result = await _userManager.RemoveFromRolesAsync(user,
                    userRoles.Except(selectedRole).ToArray());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().Description);
                    return View();
                }
                return RedirectToAction("Index");
                
            }
            ModelState.AddModelError("", "Something failed");
            return View();
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return BadRequest();
                }
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().Description);
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
