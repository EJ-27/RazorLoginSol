using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Zook
{
    public class DeleteModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DeleteModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Zookeeper Zookeeper { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zookeeper = await _context.Zookeepers.FirstOrDefaultAsync(m => m.ZookeeperId == id);

            if (zookeeper == null)
            {
                return NotFound();
            }
            else
            {
                Zookeeper = zookeeper;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zookeeper = await _context.Zookeepers
                .Include(z => z.Enclosures)  // Include Enclosures to check if they are associated
                .Include(z => z.Animals)     // Include Animals to check if they are associated
                .FirstOrDefaultAsync(z => z.ZookeeperId == id);

            if (zookeeper == null)
            {
                return NotFound();
            }

            // Check if the zookeeper has associated enclosures
            if (zookeeper.Enclosures.Any())
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this zookeeper because they are assigned to enclosures.");
                Zookeeper = zookeeper; // Re-bind zookeeper data for the page
                return Page(); // Re-render the page with the error
            }

            // Check if the zookeeper has associated animals
            if (zookeeper.Animals.Any())
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this zookeeper because they are associated with animals.");
                Zookeeper = zookeeper; // Re-bind zookeeper data for the page
                return Page(); // Re-render the page with the error
            }

            try
            {
                // Proceed with the deletion of the zookeeper
                _context.Zookeepers.Remove(zookeeper);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the error (if needed)
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the zookeeper. Please try again.");
                Zookeeper = zookeeper; // Re-bind zookeeper data
                return Page(); // Re-render the page with the error message
            }
            catch (Exception ex)
            {
                // Catch other unexpected errors
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                Zookeeper = zookeeper; // Re-bind zookeeper data
                return Page(); // Re-render the page with the error message
            }

            // Redirect to the list page after successful deletion
            return RedirectToPage("./Index");
        }

    }
}
