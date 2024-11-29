using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Admin.Personal
{
    [Authorize(Roles = "Admin")]
    public class viewPersonalModel : PageModel
    {
        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public viewPersonalModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Check if the id is valid
            if (id <= 0)
            {
                return NotFound();
            }

            Employee = await _context.Employees.FindAsync(id);
            if (Employee == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page(); // Return to the same page to show validation errors
            }

            // Find the employee to update
            var employeeToUpdate = await _context.Employees.FindAsync(Employee.EmployeeId);
            if (employeeToUpdate == null)
            {
                return NotFound();
            }

            // Update the properties
            employeeToUpdate.EmployeeFirstName = Employee.EmployeeFirstName;
            employeeToUpdate.EmployeeLastName = Employee.EmployeeLastName;
            employeeToUpdate.EmployeeAddress = Employee.EmployeeAddress;
            employeeToUpdate.EmployeePhoneNumber = Employee.EmployeePhoneNumber;
            employeeToUpdate.EmployeeDob = Employee.EmployeeDob;


            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");


        }
    }
}