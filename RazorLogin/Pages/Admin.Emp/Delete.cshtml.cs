using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Emp
{
    public class DeleteModel : PageModel
    {
        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                Employee = employee;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Dependants)
                .Include(e => e.Events)
                .Include(e => e.Manager)
                .Include(e => e.Zookeeper)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }



            // Remove the validation errors for fields we don't care about in the delete process.
            ModelState.Remove("Employee.EmployeeEmail");
            ModelState.Remove("Employee.EmployeeDob");


            // Check if the employee has a matching EmployeeId in the Manager table
            if (_context.Managers.Any(m => m.EmployeeId == employee.EmployeeId))
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this employee because they are a manager.");
            }

            // Check if the employee has a matching EmployeeId in the Zookeeper table
            if (_context.Zookeepers.Any(z => z.EmployeeId == employee.EmployeeId))
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this employee because they are a zookeeper.");
            }

            // Check if the employee has dependents
            if (employee.Dependants.Any())
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this employee because they have dependents.");
            }

            // Check if the employee has events
            if (employee.Events.Any())
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this employee because they are associated with events.");
            }

            // If any errors were added to the ModelState, return to the page
            if (!ModelState.IsValid)
            {
                // Re-bind the employee to the view model
                Employee = employee;
                return Page();
            }

            // Proceed with deleting the employee and associated user
            var user = await _userManager.FindByEmailAsync(employee.EmployeeEmail);
            _context.Employees.Remove(employee);

            try
            {
                await _context.SaveChangesAsync();

                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        // Handle errors if user deletion fails
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        Employee = employee; // Re-bind the employee to the view model
                        return Page(); // Return to the page to display errors
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                // Log the error (ex) if needed
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the employee. Please try again.");
                Employee = employee; // Re-bind the employee to the view model
                return Page(); // Return to the page to display errors
            }

            return RedirectToPage("./Index");
        }

    }
}
