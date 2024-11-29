using global::RazorLogin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Zoo.Myinfo
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        public Employee Employee { get; set; }
        public Zookeeper Zookeeper { get; set; }

        public async Task OnGetAsync()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (!string.IsNullOrEmpty(userEmail))
            {
                Employee = await _context.Employees
                    .Include(e => e.Zookeeper) // Ensuring Zookeeper data loads with Employee
                    .FirstOrDefaultAsync(e => e.EmployeeEmail == userEmail);

                // If navigation property doesn't work, load Zookeeper manually
                if (Employee != null && Employee.Zookeeper == null)
                {
                    Zookeeper = await _context.Zookeepers
                        .FirstOrDefaultAsync(z => z.EmployeeId == Employee.EmployeeId);
                }
                else
                {
                    Zookeeper = Employee?.Zookeeper;
                }
            }
        }
    }
}

