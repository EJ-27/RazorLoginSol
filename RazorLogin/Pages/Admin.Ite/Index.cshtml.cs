using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Ite
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        // Property to hold the list of items
        public IList<Item> Item { get; set; } = default!;

        // Property to hold the ShopId (optional)
        public int? ShopId { get; set; }

        // OnGetAsync method to fetch and filter items by ShopId
        public async Task OnGetAsync(int? shopId)
        {
            ShopId = shopId;

            // Start the query
            var query = _context.Items.AsQueryable();  // Start with a base IQueryable

            // Apply filter if ShopId is provided
            if (ShopId.HasValue)
            {
                query = query.Where(i => i.ShopId == ShopId.Value); // Apply filter first
            }

            // Now, apply the Include() method
            query = query.Include(i => i.Shop);  // This includes the related GiftShop

            // Execute the query
            Item = await query.ToListAsync();
        }

    }
}
