using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.CustomerEnclosures
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IList<Enclosure> Enclosure { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Enclosure = await _context.Enclosures
                .Include(e => e.Zookeeper)
                .Include(e => e.Animals)
                .ToListAsync();
        }
    }
}
