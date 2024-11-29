using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;

namespace RazorLogin.Pages.Zook.Eve
{
    public class CreateModel : PageModel
    {
        private readonly ZooDbContext _context;

        public CreateModel(ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Event Event { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Generate a unique EventId
            Random random = new Random();
            int randomSuffix = random.Next(10000, 99999);
            Event.EventId = randomSuffix;

            while (await _context.Events.AnyAsync(d => d.EventId == Event.EventId))
            {
                randomSuffix = random.Next(10000, 99999);
                Event.EventId = randomSuffix;
            }

            // Get the logged-in user's ID and set it as the EmployeeRepId
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeEmail == userEmail);

            if (employee != null)
            {
                Event.EventEmployeeRepId = employee.EmployeeId;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unable to identify the current user as an employee.");
                return Page();
            }

            // Ensure the IsDeleted column is set to false for new events
            Event.IsDeleted = false;

            try
            {
                // Check for conflicts, excluding soft-deleted events
                var conflict = await _context.Events
                    .Where(e => e.EventDate == Event.EventDate &&
                                e.EventLocation == Event.EventLocation &&
                                (e.IsDeleted == false || e.IsDeleted == null)) // Handle nullable IsDeleted
                    .FirstOrDefaultAsync();

                if (conflict != null)
                {
                    ModelState.AddModelError(string.Empty, "This event cannot be scheduled because another event is already scheduled at the same date, time, and location.");
                    return Page();
                }

                _context.Events.Add(Event);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while saving your data: {ex.InnerException?.Message ?? ex.Message}");
                return Page();
            }
        }
    }
}


