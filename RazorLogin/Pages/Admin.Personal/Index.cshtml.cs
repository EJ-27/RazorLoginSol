using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;
using System.Security.Claims;

namespace RazorLogin.Pages.Admin.Personal
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        public IList<Employee> Employees { get; set; } = new List<Employee>();

        public async Task OnGetAsync()
        {
            // Retrieve the current user's email or ID
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // or ClaimTypes.NameIdentifier
            // Log the userEmail to see what value it has
            if (string.IsNullOrEmpty(userEmail))
            {
                // Log or handle the case where the email is not found
                // For example, you might use Debug.WriteLine in development
            }

            // Fetch employee records matching the logged-in user's email
            Employees = await _context.Employees
                .Where(e => e.EmployeeEmail == userEmail)
                .ToListAsync();

            // Check if Employees is empty
            if (Employees.Count == 0)
            {
                // Log or handle the case where no employees are found
            }
        }
    }
}