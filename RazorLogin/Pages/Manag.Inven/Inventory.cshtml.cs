using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manag.Inven
{
    public class InventoryModel : PageModel
    {
        private readonly ZooDbContext _context;

        public InventoryModel(ZooDbContext context)
        {
            _context = context;
        }

        public int? ShopId { get; set; }
        public int? FoodStoreId { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();

        public async Task<IActionResult> OnGetAsync(int? shopId, int? foodStoreId)
        {
            if (shopId == null && foodStoreId == null)
            {
                return NotFound("No store ID provided");
            }

            ShopId = shopId;
            FoodStoreId = foodStoreId;

            if (ShopId.HasValue)
            {
                // Fetch items assigned to the specific store
                Items = await _context.Items
                    .Where(i => i.ShopId == ShopId)
                    .ToListAsync();
            }
            else if (FoodStoreId.HasValue)
            {
                // Fetch items assigned to the specific FoodStore
                Items = await _context.Items
                    .Where(i => i.FoodStoreId == FoodStoreId)
                    .ToListAsync();
            }

            return Page();
        }
    }
}
