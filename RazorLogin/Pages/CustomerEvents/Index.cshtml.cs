using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.CustomerEvents
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public List<Event> Event { get; set; } = new List<Event>();

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task OnGetAsync()
        {
            // Fetch events from the database, or set an empty list if none are found
            Event = await _context.Events.ToListAsync() ?? new List<Event>();
        }
    }
}
