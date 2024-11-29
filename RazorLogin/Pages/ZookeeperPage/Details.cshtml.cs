using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.ZookeeperPage
{
    public class DetailsModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DetailsModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public Zookeeper Zookeeper { get; set; } = default!;

public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Zookeeper = await _context.Zookeepers
            .Include(z => z.Employee) 
            .FirstOrDefaultAsync(m => m.ZookeeperId == id);

        if (Zookeeper == null)
        {
            return NotFound();
        }

        return Page();
    }

}
}
