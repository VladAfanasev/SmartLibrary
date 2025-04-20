using Microsoft.AspNetCore.Mvc;
using SmartLibrary.Application.Interfaces;
using SmartLibrary.Domain.Interfaces;
using SmartLibrary.Domain.Entities;
using SmartLibrary.Web.Models;

namespace SmartLibrary.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            try
            {
                // Updated to use GetAllUsersAsync() instead of GetUsersAsync()
                var users = await _userService.GetAllUsersAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");
                TempData["ErrorMessage"] = $"Error loading users: {ex.Message}";
                return View(new List<IUser>());
            }
        }

        // GET: /Users/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: /Users/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        PasswordHash = model.Password, // This should be hashed
                        IsActive = true,
                        RegistrationDate = DateTime.UtcNow,
                        RenewalDate = DateTime.UtcNow.AddYears(1),
                        MembershipTypeID = model.MembershipTypeId
                    };

                    // Handle the bool return value
                    bool success = await _userService.AddUserAsync(user);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "User added successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to add user. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding user");
                    ModelState.AddModelError("", $"Error adding user: {ex.Message}");
                }
            }

            return View(model);
        }

        // GET: /Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                MembershipTypeId = user.MembershipTypeID
            };

            return View(model);
        }

        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _userService.GetUserByIdAsync(model.Id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found";
                    return RedirectToAction(nameof(Index));
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.MembershipTypeID = model.MembershipTypeId;

                // Handle bool return value
                bool success = await _userService.UpdateUserAsync(user);
                if (success)
                {
                    TempData["SuccessMessage"] = "User updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update user. Please try again.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                ModelState.AddModelError("", $"Error updating user: {ex.Message}");
                return View(model);
            }
        }

        // GET: /Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user for deletion");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                bool success = await _userService.DeleteUserAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "User deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete user. Please try again.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                TempData["ErrorMessage"] = $"Error deleting user: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}