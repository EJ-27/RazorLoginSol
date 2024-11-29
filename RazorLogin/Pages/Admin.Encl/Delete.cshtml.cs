using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Encl
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enclosure = await _context.Enclosures
                .Include(e => e.Animals) // Include the Animals associated with this enclosure
                .FirstOrDefaultAsync(e => e.EnclosureId == id);

            if (enclosure == null)
            {
                return NotFound();
            }

            // Check if the enclosure has associated animals
            if (enclosure.Animals.Any())
            {
                // Add an error to the ModelState if animals are found
                ModelState.AddModelError(string.Empty, "Cannot delete this enclosure because it is associated with animals.");
                Enclosure = enclosure; // Re-bind enclosure data for the page
                return Page(); // Re-render the page with the error
            }

            try
            {
                // Proceed with deletion if no animals are associated
                _context.Enclosures.Remove(enclosure);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log and handle any database-related errors
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the enclosure. Please try again.");
                Enclosure = enclosure; // Re-bind enclosure data
                return Page(); // Re-render the page with the error message
            }
            catch (Exception ex)
            {
                // Catch unexpected errors
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                Enclosure = enclosure; // Re-bind enclosure data
                return Page(); // Re-render the page with the error message
            }

            // Redirect to the list page after successful deletion
            return RedirectToPage("./Index");
        }

    }
}
