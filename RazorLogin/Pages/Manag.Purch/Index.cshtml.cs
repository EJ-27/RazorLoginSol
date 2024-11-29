using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manag.Purch
{
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Purchase> Purchases { get; set; } = new List<Purchase>();

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return RedirectToPage("/Account/Login");

            // Retrieve all purchases to display
            Purchases = await _context.Purchases
            .Include(p => p.Tickets) // Assuming a navigation property exists
            .ToListAsync();

            Purchases = await _context.Purchases
                .Where(p => !_context.Tickets.Any(t => t.PurchaseId == p.PurchaseId))
                .ToListAsync();

            return Page();
        }
    }
}
