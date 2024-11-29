using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.EnclosurePage
{
    public class DeleteModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DeleteModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Enclosure Enclosure { get; set; } = default!;

        // Get the Enclosure to be deleted
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enclosure = await _context.Enclosures.FirstOrDefaultAsync(m => m.EnclosureId == id);

            if (enclosure == null)
            {
                return NotFound();
            }
            else
            {
                Enclosure = enclosure;
            }
            return Page();
        }

        // Handle the actual deletion logic
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var enclosure = await _context.Enclosures.FindAsync(id);
                if (enclosure == null)
                {
                    return NotFound();
                }

                // Check if the enclosure is assigned to any animals
                var assignedAnimals = await _context.Animals
                    .Where(a => a.EnclosureId == enclosure.EnclosureId)
                    .ToListAsync();

                if (assignedAnimals.Any())
                {
                    // If the enclosure has animals assigned, show an error message on the same page
                    ModelState.AddModelError(string.Empty, "You cannot delete an enclosure that has animals assigned to it.");
                    return Page(); // Stay on the same page with the error message
                }

                // If no animals are assigned to the enclosure, proceed with the deletion
                _context.Enclosures.Remove(enclosure);
                await _context.SaveChangesAsync();

                // Redirect to the Index page after successful deletion
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Log the exception (you can log it to a file, database, etc. in production)
                Console.Error.WriteLine($"Error deleting enclosure: {ex.Message}");

                // Add a generic error message to display on the same page
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while trying to delete the enclosure.");
                return Page();
            }
        }
    }
}
