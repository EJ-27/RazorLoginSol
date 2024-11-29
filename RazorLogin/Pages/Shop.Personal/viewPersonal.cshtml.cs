using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorLogin.Data;
using RazorLogin.Models;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Shop.Personal
{
    [Authorize]
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
        public RazorLogin.Models.Employee Employee { get; set; }

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

            employeeToUpdate.EmployeeFirstName = Employee.EmployeeFirstName;
            employeeToUpdate.EmployeeLastName = Employee.EmployeeLastName;
            employeeToUpdate.EmployeeAddress = Employee.EmployeeAddress;
            employeeToUpdate.EmployeePhoneNumber = Employee.EmployeePhoneNumber;
            employeeToUpdate.EmployeeDob = Employee.EmployeeDob;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
