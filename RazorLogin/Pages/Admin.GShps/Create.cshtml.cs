using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.GShps
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
        public GiftShop GiftShop { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Fetch the highest ShopId from the database, if any.
                var maxShopId = await _context.GiftShops
                                                .OrderByDescending(s => s.ShopId)
                                                .Select(s => s.ShopId)
                                                .FirstOrDefaultAsync();

                // Set the new ShopId to maxShopId + 1 (or 1 if no ShopId exists).
                GiftShop.ShopId = maxShopId + 1;

                // Ensure that ShopId is unique by checking if it exists in the database.
                while (await _context.GiftShops.AnyAsync(s => s.ShopId == GiftShop.ShopId))
                {
                    // Increment ShopId until it becomes unique.
                    GiftShop.ShopId++;
                }

                // Add the new GiftShop to the context and save changes.
                _context.GiftShops.Add(GiftShop);
                await _context.SaveChangesAsync();

                // Redirect to the index page if the operation is successful.
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                // Add a detailed error message to ModelState in case of database-related issues.
                ModelState.AddModelError(string.Empty, "An error occurred while saving the Gift Shop. Please try again.");
                ModelState.AddModelError(string.Empty, "Database Error: " + ex.Message);  // Optionally, log the full exception

                // You may want to log the exception for debugging purposes (for dev environments, use a logger).
                // _logger.LogError(ex, "Database error while adding a new Gift Shop");

                // Return the page with error details.
                return Page();
            }
            catch (Exception ex)
            {
                // Catch any general exception and display a message.
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);  // Optionally, log the full exception

                // Return the page with error details.
                return Page();
            }
        }

    }
}

