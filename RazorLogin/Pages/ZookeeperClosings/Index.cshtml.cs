using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.ZookeeperClosings
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        public IList<Closing> Closing { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var currentDate = DateTime.Today;

            // Retrieve the enclosures that are about to have their closings removed
            var enclosuresToUpdate = await _context.Closings
                .Where(c => c.ClosingsEnd < currentDate)
                .Select(c => c.EnclosureId)
                .Distinct()
                .ToListAsync();

            // Update their Occupancy_Status to "OPEN"
            var enclosures = await _context.Enclosures
                .Where(e => enclosuresToUpdate.Contains(e.EnclosureId))
                .ToListAsync();

            foreach (var enclosure in enclosures)
            {
                enclosure.OccupancyStatus = "OPEN";
            }

            await _context.SaveChangesAsync();

            // Delete past closings
            _context.Closings.RemoveRange(_context.Closings.Where(c => c.ClosingsEnd < currentDate));
            await _context.SaveChangesAsync();

            // Retrieve remaining closings
            Closing = await _context.Closings
                .Include(c => c.Enclosure)
                .ToListAsync();
        }
    }
}
