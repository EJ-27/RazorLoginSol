using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient; // Import the correct namespace
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Pur
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Purchase> Purchases { get; set; } = new List<Purchase>();

        public async Task OnGetAsync()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Updated SQL query to join Purchase and Ticket tables
                string query = @"
            SELECT 
                p.Purchase_ID, 
                p.Customer_ID, 
                p.Store_ID, 
                p.num_items, 
                p.Total_purchases_price, 
                p.Purchase_Method, 
                t.Ticket_ID
            FROM 
                Purchase p
            LEFT JOIN 
                Ticket t ON p.Purchase_ID = t.Purchase_ID
            ORDER BY 
                p.Customer_ID;
        ";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var purchase = new Purchase
                        {
                            PurchaseId = reader.GetInt32(reader.GetOrdinal("Purchase_ID")),
                            CustomerId = reader.IsDBNull(reader.GetOrdinal("Customer_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Customer_ID")),
                            StoreId = reader.IsDBNull(reader.GetOrdinal("Store_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Store_ID")),
                            NumItems = reader.IsDBNull(reader.GetOrdinal("num_items")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("num_items")),
                            TotalPurchasesPrice = reader.IsDBNull(reader.GetOrdinal("Total_purchases_price")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Total_purchases_price")),
                            PurchaseMethod = reader.IsDBNull(reader.GetOrdinal("Purchase_Method")) ? null : reader.GetString(reader.GetOrdinal("Purchase_Method")),
                        };

                        // Check if StoreId is null and if there's an associated ticket
                        if (!purchase.StoreId.HasValue && reader["Ticket_ID"] != DBNull.Value)
                        {
                            // Set StoreId to 'Ticket Sale' for purchases with associated tickets
                            purchase.StoreId = -1; // We will display 'Ticket Sale' in the UI for this case
                        }

                        Purchases.Add(purchase);
                    }
                }
            }
        }

    }
}
