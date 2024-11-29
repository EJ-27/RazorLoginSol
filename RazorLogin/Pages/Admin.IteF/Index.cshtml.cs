using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.IteF
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

        // Property to hold the FoodStoreId (optional)
        public int? FoodStoreId { get; set; }

        // OnGetAsync method to fetch and filter items by FoodStoreId
        public async Task OnGetAsync(int? foodStoreId)
        {
            FoodStoreId = foodStoreId;

            // Start the query
            var query = _context.Items.AsQueryable();  // Start with a base IQueryable

            // Apply filter if FoodStoreId is provided
            if (FoodStoreId.HasValue)
            {
                query = query.Where(i => i.FoodStoreId == FoodStoreId.Value); // Apply filter for food store
            }

            // Include the FoodStore to make sure related data is loaded
            query = query.Include(i => i.FoodStore);

            // Execute the query
            Item = await query.ToListAsync();
        }
    }
}
