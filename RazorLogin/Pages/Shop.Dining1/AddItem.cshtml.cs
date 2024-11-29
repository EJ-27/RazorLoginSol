using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.GShop.Dining1
{
    public class AddItemModel : PageModel
    {

        [BindProperty]
        public Item Item { get; set; } = new Item();
        public int FoodStoreId {  get; set; }

        public readonly RazorLogin.Models.ZooDbContext _context;
        public AddItemModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            Item= Item ?? new Item();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            Random random = new Random();
            int randomSuffix = random.Next(10000, 99999);
            Item.ItemId = randomSuffix;

            while (await _context.Items.AnyAsync(d => d.ItemId == Item.ItemId))
            {
                randomSuffix = random.Next(10000, 99999);
                Item.ItemId = randomSuffix;
            }

            // Insert item into the database using raw SQL
            var sqlQuery = "INSERT INTO Item (item_ID, Item_name, Item_count, Restock_date, Food_store_ID) VALUES (@ItemId, @ItemName, @ItemCount, @RestockDate, @FoodStoreId)";

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
                    command.Parameters.Add(new SqlParameter("@FoodStoreId", Item.FoodStoreId ?? (object)DBNull.Value));
                    await command.ExecuteNonQueryAsync();
                }
            }

            // Redirect to the inventory page based on which ID (shopId or foodStoreId) was used
            if (Item.ShopId.HasValue)
            {
                return RedirectToPage("./Inventory", new { FoodStoreId = Item.ShopId });
            }
            else if (Item.FoodStoreId.HasValue)
            {
                return RedirectToPage("./Inventory", new { FoodStoreId = Item.FoodStoreId });
            }
            else
            {
                ModelState.AddModelError("", "Unable to determine the correct inventory page to redirect to.");
                return Page();
            }
        }
    }
}
