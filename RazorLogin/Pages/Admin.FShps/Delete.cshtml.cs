using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.FShps
{
    public class DeleteModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DeleteModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FoodStore FoodStore { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodstore = await _context.FoodStores.FirstOrDefaultAsync(m => m.FoodStoreId == id);

            if (foodstore == null)
            {
                return NotFound();
            }
            else
            {
                FoodStore = foodstore;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodstore = await _context.FoodStores
                .FirstOrDefaultAsync(f => f.FoodStoreId == id);

            if (foodstore == null)
            {
                return NotFound();
            }

            ModelState.Remove("FoodStore.FoodStoreName");
            ModelState.Remove("FoodStore.FoodStoreLocation");

            // Check if any employees are associated with this foodstore
            var employeesAssignedToFoodStore = await _context.Employees
                .Where(e => e.FoodStoreId == foodstore.FoodStoreId)
                .ToListAsync();

            if (employeesAssignedToFoodStore.Any())
            {
                // Add an error to the ModelState to indicate that employees are still assigned
                ModelState.AddModelError(string.Empty, "Cannot delete this food store because there are employees assigned to it.");
                FoodStore = foodstore; // Re-bind the foodstore data for the page
                return Page(); // Re-render the page with the error
            }

            try
            {
                // Proceed with deletion if no employees are assigned to the food store
                _context.FoodStores.Remove(foodstore);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Handle specific database errors (e.g., foreign key violation, or other update issues)
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the food store. " + dbEx.Message);
                FoodStore = foodstore; // Re-bind foodstore data
                return Page(); // Re-render the page with the error message
            }
            catch (Exception ex)
            {
                // Catch unexpected errors
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later. " + ex.Message);
                FoodStore = foodstore; // Re-bind foodstore data
                return Page(); // Re-render the page with the error message
            }

            // Redirect to the list page after successful deletion
            return RedirectToPage("./Index");
        }
    }
}
