using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Shop.Eve
{
    public class ReportModel : PageModel
    {
        private readonly ZooDbContext _context;

        public ReportModel(ZooDbContext context)
        {
            _context = context;
        }

        // Properties to hold the date range inputs from the user
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Property to hold the list of Events along with their associated Employees
        public IList<Event> Events { get; set; } = new List<Event>();

        public async Task OnPostAsync(DateTime? startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;

            var query = @"
                SELECT e.Event_ID, e.Event_name, e.Event_start_time, e.Event_end_time, 
                       e.Event_date, e.Event_Location, emp.Employee_First_name, emp.Employee_Last_name
                FROM Event e
                LEFT JOIN Employee emp ON e.Event_employee_rep_ID = emp.Employee_ID
                WHERE e.Event_date BETWEEN @startDate AND @endDate";

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.Parameters.Add(new SqlParameter("@startDate", StartDate ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@endDate", EndDate ?? (object)DBNull.Value));

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            Events = new List<Event>();
                            while (await reader.ReadAsync())
                            {
                                var eventItem = new Event
                                {
                                    EventId = reader.GetInt32(reader.GetOrdinal("Event_ID")),
                                    EventName = reader.GetString(reader.GetOrdinal("Event_name")),
                                    EventStartTime = reader.IsDBNull(reader.GetOrdinal("Event_start_time"))
                                                     ? TimeOnly.MinValue
                                                     : TimeOnly.FromTimeSpan(reader.GetFieldValue<TimeSpan>(reader.GetOrdinal("Event_start_time"))),
                                    EventEndTime = reader.IsDBNull(reader.GetOrdinal("Event_end_time"))
                                                   ? TimeOnly.MinValue
                                                   : TimeOnly.FromTimeSpan(reader.GetFieldValue<TimeSpan>(reader.GetOrdinal("Event_end_time"))),
                                    EventDate = reader.IsDBNull(reader.GetOrdinal("Event_date"))
                                                ? (DateOnly?)null
                                                : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Event_date"))),
                                    EventLocation = reader.GetString(reader.GetOrdinal("Event_Location")),
                                    EventEmployeeRep = new Employee
                                    {
                                        EmployeeFirstName = reader.IsDBNull(reader.GetOrdinal("Employee_First_name"))
                                                            ? "N/A"
                                                            : reader.GetString(reader.GetOrdinal("Employee_First_name")),
                                        EmployeeLastName = reader.IsDBNull(reader.GetOrdinal("Employee_Last_name"))
                                                           ? "N/A"
                                                           : reader.GetString(reader.GetOrdinal("Employee_Last_name"))
                                    }
                                };

                                Events.Add(eventItem);
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
