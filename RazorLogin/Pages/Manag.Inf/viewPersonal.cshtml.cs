using global::RazorLogin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using System.Threading.Tasks;

namespace RazorLogin.Pages.ManagerInfo
{
    [Authorize(Roles = "Manager")]
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

        [BindProperty]
        public Manager Manager { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            Employee = await _context.Employees.FindAsync(id);
            if (Employee == null)
            {
                return NotFound();
            }

            Manager = await _context.Managers.FirstOrDefaultAsync(m => m.EmployeeId == Employee.EmployeeId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var employeeToUpdate = await _context.Employees.FindAsync(Employee.EmployeeId);
            if (employeeToUpdate == null)
            {
                return NotFound();
            }

            // Update the employee's personal details
            employeeToUpdate.EmployeeFirstName = Employee.EmployeeFirstName;
            employeeToUpdate.EmployeeLastName = Employee.EmployeeLastName;
            employeeToUpdate.EmployeeAddress = Employee.EmployeeAddress;
            employeeToUpdate.EmployeePhoneNumber = Employee.EmployeePhoneNumber;
            employeeToUpdate.EmployeeDob = Employee.EmployeeDob;

            if (Manager != null)
            {
                var managerToUpdate = await _context.Managers.FirstOrDefaultAsync(m => m.EmployeeId == Employee.EmployeeId);
                if (managerToUpdate != null)
                {
                    managerToUpdate.Department = Manager.Department;
                    managerToUpdate.ManagerEmploymentDate = Manager.ManagerEmploymentDate;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
