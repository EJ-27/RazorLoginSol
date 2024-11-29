using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Mana
{
    public class DeleteModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DeleteModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RazorLogin.Models.Manager Manager { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.ManagerId == id);

            if (manager == null)
            {
                return NotFound();
            }
            else
            {
                Manager = manager;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manager = await _context.Managers
                .Include(m => m.Employee) // Include the employee to get details of the manager.
                .FirstOrDefaultAsync(m => m.ManagerId == id);

            if (manager == null)
            {
                return NotFound();
            }

            ModelState.Remove("Manager.Department");

            // Check if any employees have this manager's ID as their SupervisorId
            var employeesUnderManager = await _context.Employees
                .Where(e => e.SupervisorId == manager.EmployeeId)
                .ToListAsync();

            if (employeesUnderManager.Any())
            {
                // If any employees are found, add an error and return to the page
                ModelState.AddModelError(string.Empty, "Cannot delete this manager because they are currently supervising employees.");
                Manager = manager; // Ensure manager data is available on the page
                return Page(); // Re-render the page with the error
            }

            try
            {
                // Proceed with deletion of the manager
                _context.Managers.Remove(manager);
                await _context.SaveChangesAsync();

                // Redirect to the list page after successful deletion
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                // Log the exception (use a logging framework like Serilog or NLog here)
                // For now, we are just adding a model error to display it in the view
                ModelState.AddModelError(string.Empty, "Cannot delete this manager because they are currently supervising employees.");

                // Optionally log the exception details (ex.Message, ex.InnerException, etc.)
                Console.WriteLine($"Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                // Re-render the page with the error message
                Manager = manager; // Ensure manager data is available on the page
                return Page();
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");

                // Log the exception details
                Console.WriteLine($"Unexpected error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                // Re-render the page with the error message
                Manager = manager; // Ensure manager data is available on the page
                return Page();
            }
        }

    }
}
