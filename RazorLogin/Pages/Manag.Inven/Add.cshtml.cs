using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Manag.Inven
{
    public class AddItemModel : PageModel
    {
        private readonly string _connectionString;
        private readonly ZooDbContext _context;

        public AddItemModel(ZooDbContext context)
        {
            _context = context;
            _connectionString = context.Database.GetDbConnection().ConnectionString;
        }

        [BindProperty]
        public Item Item { get; set; }
        public int? ShopId { get; set; }
        public int? FoodStoreId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? shopId, int? foodStoreId)
        {
            if (shopId == null && foodStoreId == null)
            {
                return RedirectToPage("./Inventory");
            }

            ShopId = shopId;
            FoodStoreId = foodStoreId;

            ViewData["ShopId"] = shopId;
            ViewData["FoodStoreId"] = foodStoreId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var employee = await _context.Employees
                .Include(e => e.Shop)
                .Include(e => e.FoodStore)
                .FirstOrDefaultAsync(e => e.EmployeeEmail == userEmail);

            if (employee == null || (employee.Shop == null && employee.FoodStore == null))
            {
                return NotFound("No shop assigned to the logged-in user.");
            }

            var shopId = employee.Shop?.ShopId;
            var foodStoreId = employee.FoodStore?.FoodStoreId;

            // Generate a random 5-digit Item ID
            var random = new Random();
            int randomItemId = random.Next(10000, 99999);

            var query = "INSERT INTO Item (Item_ID, Item_name, Item_count, Restock_date, Item_price, Shop_ID, Food_store_ID) " +
                        "VALUES (@itemId, @itemName, @itemCount, @restockDate, @itemPrice, @shopId, @foodStoreId)";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Parameters.Add(new SqlParameter("@itemId", randomItemId));
                    command.Parameters.Add(new SqlParameter("@itemName", Item.ItemName));
                    command.Parameters.Add(new SqlParameter("@itemCount", Item.ItemCount));
                    command.Parameters.Add(new SqlParameter("@restockDate", Item.RestockDate.ToDateTime(TimeOnly.MinValue)));
                    command.Parameters.Add(new SqlParameter("@itemPrice", Item.ItemPrice));
                    command.Parameters.Add(new SqlParameter("@shopId", shopId ?? (object)DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@foodStoreId", foodStoreId ?? (object)DBNull.Value));

                    await command.ExecuteNonQueryAsync();
                }
            }

            if (shopId.HasValue)
            {
                // Redirect to Inventory for ShopId
                return Redirect($"/Manag.Inven/Inventory?shopId={shopId}");
            }
            else if (foodStoreId.HasValue)
            {
                // Redirect to Inventory for FoodStoreId
                return Redirect($"/Manag.Inven/Inventory?foodStoreId={foodStoreId}");
            }

            // Fallback redirection if neither ShopId nor FoodStoreId is available
            return RedirectToPage("./Inventory");
        }
    }
}
