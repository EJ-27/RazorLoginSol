using System;
using System.Collections.Generic;
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
        public Animal Animal { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the animal to edit
            var animal = await _context.Animals
                .FirstOrDefaultAsync(m => m.AnimalId == id);
            if (animal == null)
            {
                return NotFound();
            }

            Animal = animal;

            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Find the employee associated with this user
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeEmail == user.Email); // Assuming UserId in Employee corresponds to Identity UserId
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

            // Set the ZookeeperId for the animal
            Animal.ZookeeperId = zookeeper.ZookeeperId;

            // Filter enclosures assigned to this zookeeper
            var enclosuresForZookeeper = await _context.Enclosures
                .Where(e => e.ZookeeperId == zookeeper.ZookeeperId)
                .ToListAsync();

            // Populate the EnclosureId dropdown with enclosure names
            ViewData["EnclosureId"] = new SelectList(enclosuresForZookeeper, "EnclosureId", "EnclosureName", Animal.EnclosureId);

            return Page();
        }




        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Animal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(Animal.AnimalId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.AnimalId == id);
        }
    }
}
