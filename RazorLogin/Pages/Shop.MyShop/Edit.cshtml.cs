using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.MyShop
{
    public class EditModel : PageModel
    {
        private readonly ZooDbContext _context;

        public EditModel(ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Item Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int itemId)
        {
            Item = await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.ItemId == itemId);

            if (Item == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Load the original item to keep its associations intact
            var itemToUpdate = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == Item.ItemId);

            if (itemToUpdate == null)
            {
                return NotFound();
            }

            // Update only the fields that need to be changed
            itemToUpdate.ItemCount = Item.ItemCount;
            itemToUpdate.RestockDate = Item.RestockDate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while saving data: {ex.Message}");
                return Page();
            }

            return RedirectToPage("./Inventory");
        }
    }
}
