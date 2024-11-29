using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Eve
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        // Renamed to Events to follow proper naming conventions for collections
        public IList<Event> Events { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Fix the Include syntax for loading related EmployeeRep data
            Events = await _context.Events
                .Include(e => e.EventEmployeeRep) // Correct lambda syntax
                .ToListAsync();
        }
    }
}
