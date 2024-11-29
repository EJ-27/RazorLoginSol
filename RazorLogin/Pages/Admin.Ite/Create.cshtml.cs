using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Ite
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public CreateModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int? ShopId { get; set; }

        [BindProperty]
        public Item Item { get; set; } = new Item();  // Explicitly initializing Item

        public IActionResult OnGet()
        {
            // Check if ShopId is available
            if (ShopId.HasValue)
            {
                // Log ShopId value for debugging
                Console.WriteLine($"ShopId is: {ShopId.Value}");
                Item.FoodStoreId = ShopId.Value;
            }
            else
            {
                Console.WriteLine("ShopId is null!");
            }


            // Populate the SelectLists for the dropdowns
            ViewData["FoodStoreId"] = new SelectList(_context.FoodStores, "FoodStoreId", "FoodStoreId");
            ViewData["ShopId"] = new SelectList(_context.GiftShops, "ShopId", "ShopId");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Generate a random unique 5-digit ItemId
            Item.ItemId = GenerateUniqueItemId();
            Item.ShopId = ShopId.Value;
            _context.Items.Add(Item);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin.Ite/Index", new { shopId = Item.ShopId });
        }

        private int GenerateUniqueItemId()
        {
            Random rand = new Random();
            int itemId;

            do
            {
                itemId = rand.Next(10000, 100000);
            } while (_context.Items.Any(i => i.ItemId == itemId));

            return itemId;
        }
    }
}
