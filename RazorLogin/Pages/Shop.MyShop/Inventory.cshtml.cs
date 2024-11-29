using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.MyShop
{
    [Authorize]
    public class InventoryModel : PageModel
    {
        private readonly ZooDbContext _context;
        private readonly string _connectionString;

        public InventoryModel(ZooDbContext context)
        {
            _context = context;
            _connectionString = context.Database.GetDbConnection().ConnectionString;
        }

        public List<Item> Items { get; set; } = new List<Item>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized();
            }

            var employee = await _context.Employees
                .Include(e => e.FoodStore)
                .Include(e => e.Shop)
                .FirstOrDefaultAsync(e => e.EmployeeEmail == userEmail);

            if (employee == null || (employee.FoodStore == null && employee.Shop == null))
            {
                return NotFound("No shop assigned to the logged-in user.");
            }

            var shopId = employee.Shop?.ShopId;
            var foodStoreId = employee.FoodStore?.FoodStoreId;
            var query = "SELECT * FROM Item WHERE (Shop_ID = @shopId OR Food_store_ID = @foodStoreId)";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.Parameters.Add(new SqlParameter("@shopId", shopId ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@foodStoreId", foodStoreId ?? (object)DBNull.Value));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var restockDate = reader.GetDateTime(reader.GetOrdinal("Restock_date"));
                                Items.Add(new Item
                                {
                                    ItemId = reader.GetInt32(reader.GetOrdinal("Item_ID")),
                                    ItemName = reader.GetString(reader.GetOrdinal("Item_name")),
                                    ItemCount = reader.GetInt32(reader.GetOrdinal("Item_count")),
                                    RestockDate = DateOnly.FromDateTime(restockDate),
                                    ItemPrice = reader.GetInt32(reader.GetOrdinal("item_price"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while fetching the inventory: {ex.Message}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int itemId)
        {
            var query = "DELETE FROM Item WHERE Item_ID = @itemId";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.Parameters.Add(new SqlParameter("@itemId", itemId));
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while deleting the item: {ex.Message}");
                return Page();
            }

            return RedirectToPage();
        }
    }
}
