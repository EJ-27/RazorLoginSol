using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Encl
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

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Generate the EnclosureId by concatenating EmployeeId and a random number
            Random random = new Random();
            int randomSuffix = random.Next(10000, 99999); // Generate a random 5-digit number
            Enclosure.EnclosureId = int.Parse($"{randomSuffix}"); // Concatenate EmployeeId and random number

            // Ensure the generated EnclosureId is unique
            while (await _context.Enclosures.AnyAsync(e => e.EnclosureId == Enclosure.EnclosureId))
            {
                randomSuffix = random.Next(10000, 99999); // Generate a new random number if the ID already exists
                Enclosure.EnclosureId = int.Parse($"{randomSuffix}");
            }

            _context.Enclosures.Add(Enclosure);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
