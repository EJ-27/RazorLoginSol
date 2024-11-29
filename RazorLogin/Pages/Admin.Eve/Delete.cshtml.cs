using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Eve
{
    public class DeleteModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DeleteModel(RazorLogin.Models.ZooDbContext context)
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

            // Fetch the Event by its ID
            var eventToDelete = await _context.Events.FirstOrDefaultAsync(m => m.EventId == id);

            if (eventToDelete == null)
            {
                return NotFound();
            }
            else
            {
                // Assign to the Event property for the Razor Page to use
                Event = eventToDelete;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the event from the database
            var eventToDelete = await _context.Events.FindAsync(id);

            if (eventToDelete == null)
            {
                return NotFound();
            }

            // Remove the event from the context
            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();

            // Redirect to the index page after deletion
            return RedirectToPage("./Index");
        }
    }
}
