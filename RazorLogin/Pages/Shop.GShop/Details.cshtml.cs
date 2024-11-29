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
    public class DetailsModel : PageModel
    {
        // This line allows the database context to access the database
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DetailsModel(RazorLogin.Models.ZooDbContext context)
        {
            // Initializing the database context
            _context = context;
        }

        // Property to hold the details of the Gift shops
        public GiftShop GiftShop { get; set; } = default!;

        // Property to hold the list of employees
        public List<RazorLogin.Models.Employee> Employee { get; set; } = new List<RazorLogin.Models.Employee>();
        public int Employeecount { get; set; }

        // Method called when the page is accessed (HTTP GET)
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound(); // This is for when the ID is not provided
            }

            // SQL query to get the gift shop details based on the ID
            var giftShopQ = "SELECT * FROM Gift_shop WHERE Shop_ID = @shopId";
            // SQL query to get all Employees that work in dining
            var employeeQ = "SELECT * FROM Employee WHERE Department = 'GIFT' AND Shop_ID = @shopId";
            var EmployeecountQ = "SELECT COUNT(*) FROM Employee WHERE Shop_ID = @shopId";

            try
            {
                using (var connection = _context.Database.GetDbConnection()) // Open the database connection
                {
                    await connection.OpenAsync(); // Opening asynchronously

                    // Execute the query to get the gift shops
                    using (var giftShopCommand = connection.CreateCommand())
                    {
                        // Set SQL command
                        giftShopCommand.CommandText = giftShopQ;
                        giftShopCommand.Parameters.Add(new SqlParameter("@shopId", id)); // Shop ID parameter

                        using (var reader = await giftShopCommand.ExecuteReaderAsync()) // Execute command and get a reader 
                        {
                            //HERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                            if (await reader.ReadAsync()) // Check if a row is returned
                            {
                                // Since I am having problems with mapping the TimeOnly data type, I will read them as strings instead and convert them to TimeOnly manually
                                var openTimeString = reader["Gift_shop_open_time"].ToString();
                                var closeTimeString = reader["Gift_shop_close_time"].ToString();

                                // Parsing the times manually
                                var openTime = TimeOnly.Parse(openTimeString);
                                var closeTime = TimeOnly.Parse(closeTimeString);

                                // Map the data from the reader to the GiftShop object
                                GiftShop = new GiftShop
                                {
                                    ShopId = reader.GetInt32(reader.GetOrdinal("Shop_ID")),
                                    GiftShopName = reader.GetString(reader.GetOrdinal("Gift_shop_Name")),
                                    GiftShopLocation = reader.GetString(reader.GetOrdinal("Gift_shop_Location")),
                                    GiftShopOpenTime = openTime,
                                    GiftShopCloseTime = closeTime
                                };
                            }
                            else
                            {
                                return NotFound(); // For when the shop is not found
                            }
                        }
                    }

                    // Execute the query to get employees that work at the gift shop
                    using (var employeeCommand = connection.CreateCommand())
                    {
                        employeeCommand.CommandText = employeeQ; // Set the SQL command for employees
                        employeeCommand.Parameters.Add(new SqlParameter("@shopId", id));

                        using (var reader = await employeeCommand.ExecuteReaderAsync())
                        {
                            // Iterate through each row returned
                            while (await reader.ReadAsync())
                            {
                                // Map from the reader to an employee object and add it to the Employee list
                                Employee.Add(new RazorLogin.Models.Employee
                                {
                                    EmployeeId = reader.GetInt32(reader.GetOrdinal("Employee_ID")),
                                    EmployeeFirstName = reader.GetString(reader.GetOrdinal("Employee_First_name")),
                                    EmployeeLastName = reader.GetString(reader.GetOrdinal("Employee_Last_name")),
                                    Department = reader.GetString(reader.GetOrdinal("Department"))
                                });
                            }
                        }
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = EmployeecountQ;
                        command.Parameters.Add(new SqlParameter("@shopId", id));

                        Employeecount = (int)await command.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex) // Catch any errors that occur
            {
                ModelState.AddModelError("", $"An error occurred while fetching data: {ex.Message}"); // Log error and put an error message
                return Page(); // Return the current page with the error message
            }
            return Page(); // Return the page with the data
        }
    }
}

