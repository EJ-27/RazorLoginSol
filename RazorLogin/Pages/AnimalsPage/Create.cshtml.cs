using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.AnimalsPage
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
        public Animal Animal { get; set; } = new Animal(); // Ensure Animal is instantiated

        public async Task<IActionResult> OnGetAsync()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Handle case where no user is logged in
                return Unauthorized();
            }

            // Find the employee associated with the logged-in user
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeEmail == user.Email);
            if (employee == null)
            {
                return NotFound();
            }

            // Find the zookeeper associated with this employee
            var zookeeper = await _context.Zookeepers
                .FirstOrDefaultAsync(z => z.EmployeeId == employee.EmployeeId);
            if (zookeeper == null)
            {
                return NotFound();
            }

            // Ensure Animal is initialized
            Animal.ZookeeperId = zookeeper.ZookeeperId;  // Set the logged-in user's ZookeeperId

            // Get enclosures assigned to this zookeeper
            var enclosuresForZookeeper = await _context.Enclosures
                .Where(e => e.ZookeeperId == zookeeper.ZookeeperId)
                .ToListAsync();

            // Populate Enclosure dropdown with Enclosure names
            ViewData["EnclosureId"] = new SelectList(enclosuresForZookeeper, "EnclosureId", "EnclosureName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate the Enclosure dropdown in case of validation failure
                var curruser = await _userManager.GetUserAsync(User);
                var curremployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.EmployeeEmail == curruser.Email);
                var currzookeeper = await _context.Zookeepers
                    .FirstOrDefaultAsync(z => z.EmployeeId == curremployee.EmployeeId);
                var enclosuresForZookeeper = await _context.Enclosures
                    .Where(e => e.ZookeeperId == currzookeeper.ZookeeperId)
                    .ToListAsync();

                ViewData["EnclosureId"] = new SelectList(enclosuresForZookeeper, "EnclosureId", "EnclosureName");
                return Page();
            }

            // Retrieve the Zookeeper for the logged-in user
            var user = await _userManager.GetUserAsync(User);
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeEmail == user.Email);
            var zookeeper = await _context.Zookeepers
                .FirstOrDefaultAsync(z => z.EmployeeId == employee.EmployeeId);

            if (zookeeper == null)
            {
                ModelState.AddModelError(string.Empty, "Zookeeper not found.");
                return Page();
            }

            // Generate a random 5-digit number
            Random random = new Random();
            int randomSuffix = random.Next(10000, 99999);  // Generate a 5-digit random number

            // Concatenate the Zookeeper's EmployeeId and the random number to form the AnimalId
            int animalId = int.Parse($"{zookeeper.EmployeeId}{randomSuffix}");

            // Check if the generated AnimalId already exists
            while (await _context.Animals.AnyAsync(a => a.AnimalId == animalId))
            {
                // If it exists, generate a new random number
                randomSuffix = random.Next(10000, 99999);
                animalId = int.Parse($"{zookeeper.EmployeeId}{randomSuffix}");
            }

            // Assign the generated AnimalId and ZookeeperId
            Animal.AnimalId = animalId;
            Animal.ZookeeperId = zookeeper.ZookeeperId;

            // Add the new animal to the context
            _context.Animals.Add(Animal);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

    }
}
