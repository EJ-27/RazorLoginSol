using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Emp
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

        public IList<Employee> Employee { get; set; } = default!;
        public Dictionary<string, IList<string>> EmployeeRoles { get; set; } = new Dictionary<string, IList<string>>();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string RoleSearchTerm { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            IQueryable<Employee> query = _context.Employees
                .Include(e => e.FoodStore)
                .Include(e => e.Shop)
                .Include(e => e.Supervisor);

            // Filter by employee details (first name, last name, ID)
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(e => e.EmployeeFirstName.Contains(SearchTerm) ||
                                          e.EmployeeLastName.Contains(SearchTerm) ||
                                          e.EmployeeId.ToString().Contains(SearchTerm));
            }

            // Get all users to filter by roles
            var allUsers = await _userManager.Users.ToListAsync();
            var roleFilteredEmployees = new List<Employee>();

            if (!string.IsNullOrEmpty(RoleSearchTerm))
            {
                foreach (var user in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Any(role => role.Contains(RoleSearchTerm)))
                    {
                        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeEmail == user.Email);
                        if (employee != null && !roleFilteredEmployees.Contains(employee))
                        {
                            roleFilteredEmployees.Add(employee);
                        }
                    }
                }
            }

            // Combine the filtered employees by role with the previous query
            var employeeList = await query.ToListAsync();

            // If there's a role search term, filter the combined list to include only those that match
            if (roleFilteredEmployees.Any())
            {
                Employee = employeeList.Intersect(roleFilteredEmployees).ToList();
            }
            else
            {
                Employee = employeeList; // Just use the initial query results if no role search
            }

            // Populate employee roles
            foreach (var employee in Employee)
            {
                if (!string.IsNullOrEmpty(employee.EmployeeEmail))
                {
                    var user = await _userManager.FindByEmailAsync(employee.EmployeeEmail);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        EmployeeRoles[employee.EmployeeEmail] = roles.ToList();
                    }
                }
            }
        }
    }
}