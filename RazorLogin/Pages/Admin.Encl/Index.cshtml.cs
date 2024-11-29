using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Encl
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        // The list of Enclosures to display
        public IList<Enclosure> Enclosure { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Eager load Enclosures with Zookeeper and their related Employee data
            Enclosure = await _context.Enclosures
                .Include(e => e.Zookeeper)            // Include the Zookeeper for each Enclosure
                    .ThenInclude(z => z.Employee)    // Include the related Employee (to get FirstName, LastName)
                .ToListAsync();
        }
    }
}
