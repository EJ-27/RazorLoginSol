using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Ite
{
    public class EditModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public EditModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Item Item { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }
            Item = item;

            // Add a "null" option for ShopId (representing no shop selected)
            var shopList = _context.GiftShops
                .Select(s => new SelectListItem
                {
                    Value = s.ShopId.ToString(),
                    Text = s.GiftShopName
                })
                .ToList();
            shopList.Insert(0, new SelectListItem { Value = "", Text = "Select a Gift Shop" });  // Default option for null

            ViewData["ShopId"] = new SelectList(shopList, "Value", "Text", Item.ShopId?.ToString());

            // Add a "null" option for FoodStoreId (representing no food store selected)
            var foodStoreList = _context.FoodStores
                .Select(f => new SelectListItem
                {
                    Value = f.FoodStoreId.ToString(),
                    Text = f.FoodStoreName
                })
                .ToList();
            foodStoreList.Insert(0, new SelectListItem { Value = "", Text = "Select a Food Store" });  // Default option for null

            ViewData["FoodStoreId"] = new SelectList(foodStoreList, "Value", "Text", Item.FoodStoreId?.ToString());

            return Page();
        }


        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(Item.ItemId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Admin.Ite/Index", new { shopId = Item.ShopId });
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.ItemId == id);
        }
    }
}
