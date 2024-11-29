using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using Microsoft.AspNetCore.Identity;
using global::RazorLogin.Models;

namespace RazorLogin.Pages.Shop.Deps
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
        public Dependant Dependent { get; set; } = new Dependant();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeEmail == user.Email);
            if (employee == null)
            {
                return NotFound();
            }

            Dependent = await _context.Dependants.FirstOrDefaultAsync(d => d.DepndantId == id && d.EmployeeId == employee.EmployeeId);
            if (Dependent == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var existingDependent = await _context.Dependants.FindAsync(Dependent.DepndantId);
            if (existingDependent == null)
            {
                return NotFound();
            }

            _context.Dependants.Remove(existingDependent);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }

}
