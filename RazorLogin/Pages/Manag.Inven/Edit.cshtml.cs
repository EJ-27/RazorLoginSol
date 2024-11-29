using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manag.Inven
{
    public class EditModel : PageModel
    {

        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Item Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserEmail = currentUser?.Email;

            if (string.IsNullOrEmpty(currentUserEmail))
                return RedirectToPage("/Account/Login");

            // Find the item and ensure it's within the manager's assigned store
            Item = await _context.Items.FirstOrDefaultAsync(m => m.ItemId == id);
            if (Item == null) return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeEmail == currentUserEmail);

            if (employee?.ShopId != Item.ShopId && employee?.FoodStoreId != Item.FoodStoreId)
                return Unauthorized();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Load the item from the database to ensure it exists and to attach it to the context
            var itemToUpdate = await _context.Items.FirstOrDefaultAsync(m => m.ItemId == id);
            if (itemToUpdate == null)
            {
                return NotFound();
            }

            // Attempt to update properties on itemToUpdate with values from the Item model bound to the page
            if (await TryUpdateModelAsync<Item>(
                itemToUpdate,
                "Item",
                i => i.ItemName,
                i => i.ItemCount,
                i => i.RestockDate,
                i => i.ItemPrice)) // Note: Ensure this property matches the exact name in your model
            {
                try
                {
                    await _context.SaveChangesAsync();

                    if (itemToUpdate.ShopId.HasValue)
                    {
                        // Redirect to Inventory for ShopId
                        return Redirect($"/Manag.Inven/Inventory?shopId={itemToUpdate.ShopId}");
                    }
                    else if (itemToUpdate.FoodStoreId.HasValue)
                    {
                        // Redirect to Inventory for FoodStoreId
                        return Redirect($"/Manag.Inven/Inventory?foodStoreId={itemToUpdate.FoodStoreId}");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Check if the item still exists in the database
                    if (!_context.Items.Any(e => e.ItemId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Page();
        }
    }
}