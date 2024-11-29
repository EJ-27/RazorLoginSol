using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Manag.Tick
{
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        public IList<Ticket> Tickets { get; set; } = new List<Ticket>();

        public async Task OnGetAsync()
        {
            Tickets = await _context.Tickets.ToListAsync();
        }
    }
}
