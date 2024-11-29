using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using RazorLogin.Models;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Admin.Pur
{
    public class DeleteModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public DeleteModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public Purchase Purchase { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Query to get the purchase record by PurchaseId
                string query = "SELECT * FROM Purchase WHERE Purchase_ID = @PurchaseId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PurchaseId", id);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        Purchase = new Purchase
                        {
                            PurchaseId = reader.GetInt32(reader.GetOrdinal("Purchase_ID")),
                            CustomerId = reader.IsDBNull(reader.GetOrdinal("Customer_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Customer_ID")),
                            StoreId = reader.IsDBNull(reader.GetOrdinal("Store_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Store_ID")),
                            NumItems = reader.IsDBNull(reader.GetOrdinal("num_items")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("num_items")),
                            TotalPurchasesPrice = reader.IsDBNull(reader.GetOrdinal("Total_purchases_price")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Total_purchases_price")),
                            PurchaseMethod = reader.IsDBNull(reader.GetOrdinal("Purchase_Method")) ? null : reader.GetString(reader.GetOrdinal("Purchase_Method")),
                        };
                    }
                }
            }

            if (Purchase == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                try
                {
                    // Query to check if the purchase is referenced by any other table (e.g., foreign keys)
                    string checkQuery = "SELECT COUNT(*) FROM Ticket WHERE Purchase_ID = @PurchaseId";  // Example of checking related data
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@PurchaseId", id);
                        int relatedRecords = (int)await checkCommand.ExecuteScalarAsync();

                        if (relatedRecords > 0)
                        {
                            // Add an error to the ModelState if the purchase cannot be deleted (due to foreign key constraints)
                            ModelState.AddModelError(string.Empty, "Cannot delete this purchase because it is a ticket. Delete matching purchaseID from Ticket table first.");
                            return Page(); // Re-render the page with the error
                        }
                    }

                    // Query to delete the purchase record
                    string deleteQuery = "DELETE FROM Purchase WHERE Purchase_ID = @PurchaseId";
                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PurchaseId", id);
                        await command.ExecuteNonQueryAsync();
                    }

                    // If deletion was successful, redirect to the index page
                    return RedirectToPage("Index");
                }
                catch (SqlException ex)
                {
                    // Handle any database-specific errors (e.g., foreign key violations)
                    ModelState.AddModelError(string.Empty, "An error occurred while trying to delete this purchase. " + ex.Message);
                    return Page(); // Re-render the page with the error
                }
                catch (Exception ex)
                {
                    // Handle any unexpected errors
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                    return Page(); // Re-render the page with the error
                }
            }
        }
    }
}
