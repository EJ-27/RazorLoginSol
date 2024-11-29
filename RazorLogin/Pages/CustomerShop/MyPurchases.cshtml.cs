using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.CustomerShop
{
    public class MyPurchasesModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MyPurchasesModel(RazorLogin.Models.ZooDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IList<Purchase> Purchases { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Get the logged-in user's email
            var userEmail = _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return;
            }

            // Retrieve the customer's purchases by email
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerEmail == userEmail);
            if (customer != null)
            {
                // Retrieve purchases with a valid ItemName
                Purchases = await _context.Purchases
                    .Where(p => p.CustomerId == customer.CustomerId && !string.IsNullOrEmpty(p.ItemName))
                    .ToListAsync();
            }
        }
    }
}
