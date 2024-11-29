using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.AnimalsPage
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(RazorLogin.Models.ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Animal> Animal { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Find the employee linked to this user
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeEmail == user.Email);
                if (employee != null)
                {
                    // Find the zookeeper linked to the employee
                    var zookeeper = await _context.Zookeepers.FirstOrDefaultAsync(z => z.EmployeeId == employee.EmployeeId);
                    if (zookeeper != null)
                    {
                        // Fetch only the animals assigned to this zookeeper
                        Animal = await _context.Animals
                            .Where(a => a.ZookeeperId == zookeeper.ZookeeperId) // Filter by ZookeeperId
                            .Include(a => a.Enclosure)  // Include the Enclosure details
                            .Include(a => a.Zookeeper)  // Include the Zookeeper details
                            .ToListAsync();
                    }
                }
            }
        }
    }
}
