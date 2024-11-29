using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Tick
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
        ViewData["PurchaseId"] = new SelectList(_context.Purchases, "PurchaseId", "PurchaseId");
            return Page();
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = default!;

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
            int TicketId = int.Parse($"{randomSuffix}");

            // Check if the generated AnimalId already exists
            while (await _context.Tickets.AnyAsync(a => a.TicketId == TicketId))
            {
                // If it exists, generate a new random number
                randomSuffix = random.Next(10000, 99999);
                TicketId = int.Parse($"{randomSuffix}");
            }



            _context.Tickets.Add(Ticket);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
