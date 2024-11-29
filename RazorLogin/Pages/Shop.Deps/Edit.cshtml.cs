using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using Microsoft.AspNetCore.Identity;
using global::RazorLogin.Models;

namespace RazorLogin.Pages.Shop.Deps
{
    public class EditModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(RazorLogin.Models.ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Dependant Dependent { get; set; } = new Dependant();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Find the employee by their email
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeEmail == user.Email);
            if (employee == null)
            {
                return NotFound();
            }

            // Find the dependent by the user-assigned DependantId
            Dependent = await _context.Dependants.FirstOrDefaultAsync(d => d.DepndantId == id && d.EmployeeId == employee.EmployeeId);
            if (Dependent == null)
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

            // Find the existing dependent using the DependantId provided by the user
            var existingDependent = await _context.Dependants.FirstOrDefaultAsync(d => d.DepndantId == Dependent.DepndantId);
            if (existingDependent == null)
            {
                return NotFound();
            }

            // Update properties
            existingDependent.DependentName = Dependent.DependentName;
            existingDependent.DependentDob = Dependent.DependentDob;
            existingDependent.Relationship = Dependent.Relationship;
            existingDependent.HealthcareTier = Dependent.HealthcareTier;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }

}

