using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorLogin.Models;

namespace RazorLogin.Pages.CustomerEnclosures
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public CreateModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["ZookeeperId"] = new SelectList(_context.Zookeepers, "ZookeeperId", "ZookeeperId");
            return Page();
        }

        [BindProperty]
        public Enclosure Enclosure { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Enclosures.Add(Enclosure);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
