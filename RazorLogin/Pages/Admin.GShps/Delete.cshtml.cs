using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.GShps
{
    public class DeleteModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DeleteModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GiftShop GiftShop { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giftshop = await _context.GiftShops.FirstOrDefaultAsync(m => m.ShopId == id);

            if (giftshop == null)
            {
                return NotFound();
            }
            else
            {
                GiftShop = giftshop;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giftshop = await _context.GiftShops
                .FirstOrDefaultAsync(g => g.ShopId == id);

            if (giftshop == null)
            {
                return NotFound();
            }

            // Check if any employees are associated with this giftshop
            var employeesAssignedToGiftShop = await _context.Employees
                .Where(e => e.ShopId == giftshop.ShopId)
                .ToListAsync();

            if (employeesAssignedToGiftShop.Any())
            {
                // Add an error to the ModelState if employees are found
                ModelState.AddModelError(string.Empty, "Cannot delete this gift shop because there are employees assigned to it.");
                GiftShop = giftshop; // Re-bind giftshop data for the page
                return Page(); // Re-render the page with the error
            }

            try
            {
                // Proceed with deletion if no employees are assigned
                _context.GiftShops.Remove(giftshop);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Handle specific database errors (e.g., foreign key violation, or other update issues)
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the gift shop. " + dbEx.Message);
                GiftShop = giftshop; // Re-bind giftshop data
                return Page(); // Re-render the page with the error message
            }
            catch (Exception ex)
            {
                // Catch unexpected errors
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later. " + ex.Message);
                GiftShop = giftshop; // Re-bind giftshop data
                return Page(); // Re-render the page with the error message
            }

            // Redirect to the list page after successful deletion
            return RedirectToPage("./Index");
        }
    }
}
