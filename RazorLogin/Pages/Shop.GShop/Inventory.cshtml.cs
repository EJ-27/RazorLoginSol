using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.GShop
{
    public class InventoryModel : PageModel
    {
        private readonly string _connectionString;

        public InventoryModel(RazorLogin.Models.ZooDbContext context)
        {
            // Fetch the connection string from the context options
            _connectionString = context.Database.GetDbConnection().ConnectionString;
        }

        public int ShopId { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
        

        public async Task<IActionResult> OnGetAsync(int? shopId)
        {
            if (shopId == null)
            {
                return NotFound(); // No shop ID provided
            }

            ShopId = shopId.Value;

            var inventoryQuery = "SELECT * FROM Item WHERE Shop_ID = @shopId";

            try
            {
                // Create a new SQL connection using the connection string
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Fetch inventory items based on Shop_ID
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = inventoryQuery;
                        command.Parameters.Add(new SqlParameter("@shopId", shopId));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                // Fetch RestockDate as DateTime and convert to DateOnly
                                var restockDate = reader.GetDateTime(reader.GetOrdinal("Restock_date"));
                                var dateOnly = DateOnly.FromDateTime(restockDate);

                                Items.Add(new Item
                                {
                                    ItemId = reader.GetInt32(reader.GetOrdinal("Item_ID")),
                                    ItemName = reader.GetString(reader.GetOrdinal("Item_name")),
                                    ItemCount = reader.GetInt32(reader.GetOrdinal("Item_count")),
                                    RestockDate = dateOnly,
                                    ShopId = reader.GetInt32(reader.GetOrdinal("Shop_ID")),
                                    FoodStoreId = reader.IsDBNull(reader.GetOrdinal("Food_store_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Food_store_ID")),
                                    ItemPrice= reader.GetInt32(reader.GetOrdinal("item_price"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex) // Catch any errors that occur
            {
                ModelState.AddModelError("", $"An error occurred while fetching the inventory: {ex.Message}");
                return Page(); // Return current page with the error message
            }

            return Page(); // Return the page with the data
        }
        public async Task<IActionResult> OnPostDeleteAsync(int itemId, int shopId)
        {
            var query = "DELETE FROM Item WHERE Item_ID = @itemId AND Shop_ID = @shopId";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.Parameters.Add(new SqlParameter("@itemId", itemId));
                        command.Parameters.Add(new SqlParameter("@shopId", shopId));

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while deleting the item: {ex.Message}");
                return Page();
            }

            // Redirect back to the inventory page after deletion
            return RedirectToPage("./Inventory", new { shopId });
        }
    }
}