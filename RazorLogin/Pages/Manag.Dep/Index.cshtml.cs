
using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Manag.Dep
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


        public IList<Dependant> Dependant { get;set; } = new List<Dependant>();


        public async Task OnGetAsync()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Find the employee by their email
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeEmail == user.Email);
                if (employee != null)
                {
                    // Fetch dependents linked to the employee
                    Dependant = await _context.Dependants
                        .Where(d => d.EmployeeId == employee.EmployeeId)
                        .ToListAsync();
                }
            }
        }
    }

}

