using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorLogin.Pages.EnclosurePage
{
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; // UserManager to access the logged-in user

        public IndexModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Enclosure> Enclosure { get; set; } = new List<Enclosure>();

        public async Task OnGetAsync()
        {
            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Find the employee by their email
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeEmail == user.Email);
                if (employee != null)
                {
                    // Find the corresponding zookeeper linked to this employee
                    var zookeeper = await _context.Zookeepers
                        .FirstOrDefaultAsync(z => z.EmployeeId == employee.EmployeeId);

                    if (zookeeper != null)
                    {
                        // Fetch the enclosures assigned to this zookeeper
                        Enclosure = await _context.Enclosures
                            .Where(e => e.ZookeeperId == zookeeper.ZookeeperId)
                            .Include(e => e.Zookeeper) // Include Zookeeper for each Enclosure
                                .ThenInclude(z => z.Employee) // Include the related Employee data (for FirstName, LastName)
                            .ToListAsync();
                    }
                }
            }
        }
    }
}
