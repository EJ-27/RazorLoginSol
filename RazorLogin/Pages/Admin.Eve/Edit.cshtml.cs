using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Eve
{
    public class EditModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public EditModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Event Event { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventToEdit = await _context.Events
                .Include(e => e.EventEmployeeRep)  // Include related Employee data
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (eventToEdit == null)
            {
                return NotFound();
            }

            Event = eventToEdit;

            // Populate the select list for Employee reps using ViewData
            ViewData["EventEmployeeRepId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeName", Event.EventEmployeeRepId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Ensure validation errors are shown
                return Page();
            }

            try
            {
                // Attach the event entity and mark it as modified
                _context.Attach(Event).State = EntityState.Modified;

                // Update the Employee reference
                var eventToUpdate = await _context.Events
                    .Include(e => e.EventEmployeeRep)  // Ensure the Employee relation is loaded
                    .FirstOrDefaultAsync(e => e.EventId == Event.EventId);

                if (eventToUpdate == null)
                {
                    return NotFound();
                }

                // Copy updated values from the form to the entity
                eventToUpdate.EventName = Event.EventName;
                eventToUpdate.EventStartTime = Event.EventStartTime;
                eventToUpdate.EventEndTime = Event.EventEndTime;
                eventToUpdate.EventDate = Event.EventDate;
                eventToUpdate.EventLocation = Event.EventLocation;
                eventToUpdate.EventEmployeeRepId = Event.EventEmployeeRepId;

                // Save changes
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(Event.EventId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
