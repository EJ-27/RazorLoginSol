using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.EnclosurePage
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(RazorLogin.Models.ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Enclosure Enclosure { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Populate the Zookeeper dropdown (if necessary, can be optional depending on the design)
            ViewData["ZookeeperId"] = new SelectList(_context.Zookeepers.ToList(), "ZookeeperId", "ZookeeperId");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate the dropdown on postback
                ViewData["ZookeeperId"] = new SelectList(_context.Zookeepers.ToList(), "ZookeeperId", "ZookeeperId");
                return Page();
            }

            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            // Find the employee associated with the logged-in user by email
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeEmail == user.Email);
            if (employee == null)
            {
                ModelState.AddModelError(string.Empty, "Employee not found.");
                return Page();
            }

            // Find the zookeeper associated with the employee
            var zookeeper = await _context.Zookeepers.FirstOrDefaultAsync(z => z.EmployeeId == employee.EmployeeId);
            if (zookeeper == null)
            {
                ModelState.AddModelError(string.Empty, "Zookeeper not found.");
                return Page();
            }

            // Generate the EnclosureId by concatenating EmployeeId and a random number
            int employeeId = employee.EmployeeId;
            Random random = new Random();
            int randomSuffix = random.Next(10000, 99999); // Generate a random 5-digit number
            Enclosure.EnclosureId = int.Parse($"{employeeId}{randomSuffix}"); // Concatenate EmployeeId and random number

            // Ensure the generated EnclosureId is unique
            while (await _context.Enclosures.AnyAsync(e => e.EnclosureId == Enclosure.EnclosureId))
            {
                randomSuffix = random.Next(10000, 99999); // Generate a new random number if the ID already exists
                Enclosure.EnclosureId = int.Parse($"{employeeId}{randomSuffix}");
            }

            // Assign the ZookeeperId (already determined by employee) to the enclosure
            Enclosure.ZookeeperId = zookeeper.ZookeeperId;

            // Add the enclosure to the context and save changes
            _context.Enclosures.Add(Enclosure);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index"); // Redirect to the Index page after successful creation
        }
    }
}
