using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Shop.Personal
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        public RazorLogin.Models.Employee Employee { get; set; }
        public string ManagerName { get; set; }

        public async Task OnGetAsync()
        {
            // Retrieve the current user's email or ID
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                // Handle missing email case
                return;
            }

            // Fetch employee record matching the logged-in user's email
            Employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeEmail == userEmail);

            if (Employee != null && Employee.SupervisorId != null)
            {
                // Fetch the manager’s name using the Supervisor ID
                var manager = await _context.Managers
                    .Where(m => m.ManagerId == Employee.SupervisorId)
                    .Join(_context.Employees,
                          m => m.EmployeeId,
                          e => e.EmployeeId,
                          (m, e) => new { e.EmployeeFirstName, e.EmployeeLastName })
                    .FirstOrDefaultAsync();

                if (manager != null)
                {
                    ManagerName = $"{manager.EmployeeFirstName} {manager.EmployeeLastName}";
                }
                else
                {
                    ManagerName = "No manager assigned";
                }
            }
            else
            {
                ManagerName = "No manager assigned";
            }
        }
    }
}
