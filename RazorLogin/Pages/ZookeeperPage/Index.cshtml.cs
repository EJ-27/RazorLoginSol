using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorLogin.Pages.ZookeeperPage
{
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        // Property to hold the list of Zookeepers along with their associated Employees
        public List<Zookeeper> Zookeepers { get; set; } = new List<Zookeeper>();

        public async Task OnGetAsync()
        {
            var query = @"
                SELECT z.Zookeeper_ID, z.Assigned_department, z.Training_renewal_date, 
                       z.Last_training_date, z.Num_Assigned_cages, 
                       e.Employee_First_name, e.Employee_Last_name
                FROM Zookeeper z
                LEFT JOIN Employee e ON z.Employee_ID = e.Employee_ID";

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            Zookeepers = new List<Zookeeper>();
                            while (await reader.ReadAsync())
                            {
                                var zookeeper = new Zookeeper
                                {
                                    ZookeeperId = reader.GetInt32(reader.GetOrdinal("Zookeeper_ID")),
                                    AssignedDepartment = reader.GetString(reader.GetOrdinal("Assigned_department")),
                                    TrainingRenewalDate = reader.IsDBNull(reader.GetOrdinal("Training_renewal_date"))
                                                          ? (DateOnly?)null
                                                          : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Training_renewal_date"))),
                                    LastTrainingDate = reader.IsDBNull(reader.GetOrdinal("Last_training_date"))
                                                       ? (DateOnly?)null
                                                       : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Last_training_date"))),
                                    NumAssignedCages = reader.IsDBNull(reader.GetOrdinal("Num_Assigned_cages"))
                                                       ? (short?)null
                                                       : reader.GetInt16(reader.GetOrdinal("Num_Assigned_cages")),
                                    Employee = new Employee
                                    {
                                        EmployeeFirstName = reader.IsDBNull(reader.GetOrdinal("Employee_First_name"))
                                                            ? "N/A"
                                                            : reader.GetString(reader.GetOrdinal("Employee_First_name")),
                                        EmployeeLastName = reader.IsDBNull(reader.GetOrdinal("Employee_Last_name"))
                                                           ? "N/A"
                                                           : reader.GetString(reader.GetOrdinal("Employee_Last_name"))
                                    }
                                };

                                Zookeepers.Add(zookeeper);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }
    }
}
