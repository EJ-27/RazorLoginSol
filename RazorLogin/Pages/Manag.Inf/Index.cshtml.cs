using global::RazorLogin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manag.Inf
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        public List<Manager> Managers { get; set; } = new List<Manager>();

        public async Task OnGetAsync()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (!string.IsNullOrEmpty(userEmail))
            {
                // Load Employee based on the logged-in user's email
                Managers = await _context.Managers
                    .Include(e => e.Employee) // Load associated Manager if it exists
                    .ToListAsync();


            }
            }
        }
    }
