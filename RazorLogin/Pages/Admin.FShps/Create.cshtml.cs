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
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public CreateModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public FoodStore FoodStore { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Fetch the highest FoodStoreId from the database, if any
                var maxFoodStoreId = await _context.FoodStores
                                                    .OrderByDescending(s => s.FoodStoreId)
                                                    .Select(s => s.FoodStoreId)
                                                    .FirstOrDefaultAsync();

                // Set the new FoodStoreId to the next available number
                FoodStore.FoodStoreId = maxFoodStoreId + 1;

                // Ensure that the FoodStoreId is unique by checking if it already exists
                while (await _context.FoodStores.AnyAsync(s => s.FoodStoreId == FoodStore.FoodStoreId))
                {
                    // If it already exists, increment the FoodStoreId
                    FoodStore.FoodStoreId++;
                }

                // Add the new FoodStore to the context and save changes
                _context.FoodStores.Add(FoodStore);
                await _context.SaveChangesAsync();

                // Redirect to the index page if the operation is successful
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                // Catch any database-related errors and display the message
                ModelState.AddModelError(string.Empty, "An error occurred while saving the Food Store. Please try again.");
                ModelState.AddModelError(string.Empty, "Database Error: " + ex.Message);

                // Optionally, log the exception here for further debugging
                // _logger.LogError(ex, "Error while adding FoodStore");

                return Page();
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);

                return Page();
            }
        }
    }
}
