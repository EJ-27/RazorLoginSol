using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Mana
{
    public class EmpListModel : PageModel
    {
      
            private readonly ZooDbContext _context;

            public EmpListModel(ZooDbContext context)
            {
                _context = context;
            }

            public int ManagerId { get; set; }
            public IList<Employee> Employees { get; set; } = new List<Employee>();

            public async Task OnGetAsync(int managerId)
            {
                // Set the ManagerId for the page
                ManagerId = managerId;

                // Fetch all employees whose SupervisorId matches the ManagerId
                Employees = await _context.Employees
                    .Where(e => e.SupervisorId == managerId)
                    .Include(e => e.Manager) // Optional: Include manager data if needed
                    .ToListAsync();
            }
        }
    }


