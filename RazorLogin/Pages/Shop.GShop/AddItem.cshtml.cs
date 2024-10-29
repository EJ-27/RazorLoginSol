using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.GShop
{
    public class AddItemModel : PageModel
    {

        [BindProperty]
        public Item Item { get; set; } = new Item();

        public readonly RazorLogin.Models.ZooDbContext _context;
        public AddItemModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            Item = Item ?? new Item();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Insert item into the database using raw SQL
            var sqlQuery = "INSERT INTO item(item_ID, Item_name, Item_count, Restock_date, Shop_ID) VALUES (@ItemId, @ItemName, @ItemCount, @RestockDate, @ShopId)";

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlQuery;
                    command.Parameters.Add(new SqlParameter("@ItemId", Item.ItemId));
                    command.Parameters.Add(new SqlParameter("@ItemName", Item.ItemName));
                    command.Parameters.Add(new SqlParameter("@ItemCount", Item.ItemCount));
                    command.Parameters.Add(new SqlParameter("@RestockDate", Item.RestockDate));
                    command.Parameters.Add(new SqlParameter("@ShopId", Item.ShopId ?? (object)DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@FoodStoreId", Item.FoodStoreId ?? (object)DBNull.Value));

                    await command.ExecuteNonQueryAsync();
                }
            }

            // Redirect to the inventory page based on which ID (shopId or foodStoreId) was used
            if (Item.ShopId.HasValue)
            {
                return RedirectToPage("./Inventory", new { shopId = Item.ShopId });
            }
            else if (Item.FoodStoreId.HasValue)
            {
                return RedirectToPage("./Inventory", new { shopId = Item.FoodStoreId });
            }
            else
            {
                ModelState.AddModelError("", "Unable to determine the correct inventory page to redirect to.");
                return Page();
            }
        }
    }
}