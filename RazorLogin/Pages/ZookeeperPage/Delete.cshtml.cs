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
    public class DeleteModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DeleteModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Zookeeper Zookeeper { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zookeeper = await _context.Zookeepers.FirstOrDefaultAsync(m => m.ZookeeperId == id);

            if (zookeeper == null)
            {
                return NotFound();
            }
            else
            {
                Zookeeper = zookeeper;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zookeeper = await _context.Zookeepers.FindAsync(id);
            if (zookeeper != null)
            {
                Zookeeper = zookeeper;
                _context.Zookeepers.Remove(Zookeeper);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
