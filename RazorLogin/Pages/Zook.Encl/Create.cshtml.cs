using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorLogin.Models;

namespace RazorLogin.Pages.Zook.Encl
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
            // Populate dropdown list for ZookeeperId
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

            // Add enclosure to the database
            _context.Enclosures.Add(Enclosure);
            await _context.SaveChangesAsync();

            // Redirect back to Index page after successful creation
            return RedirectToPage("./Index");
        }
    }
}
