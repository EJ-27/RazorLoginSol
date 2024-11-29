using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Anim
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
        ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "EnclosureId", "EnclosureId");
        ViewData["ZookeeperId"] = new SelectList(_context.Zookeepers, "ZookeeperId", "ZookeeperId");
            return Page();
        }

        [BindProperty]
        public Animal Animal { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Generate a random 5-digit number
            Random random = new Random();
            int randomSuffix = random.Next(10000, 99999);  // Generate a 5-digit random number

            // Concatenate the Zookeeper's EmployeeId and the random number to form the AnimalId
            int animalId = int.Parse($"{randomSuffix}");

            // Check if the generated AnimalId already exists
            while (await _context.Animals.AnyAsync(a => a.AnimalId == animalId))
            {
                // If it exists, generate a new random number
                randomSuffix = random.Next(10000, 99999);
                animalId = int.Parse($"{randomSuffix}");
            }

            // Assign the generated AnimalId to the Animal object
            Animal.AnimalId = animalId;

            _context.Animals.Add(Animal);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
