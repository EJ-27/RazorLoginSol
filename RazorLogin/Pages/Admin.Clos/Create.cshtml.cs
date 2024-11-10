﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Clos
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public CreateModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        // Populate the Enclosure dropdown list
        public IActionResult OnGet()
        {
            ViewData["EnclosureId"] = new SelectList(_context.Enclosures, "EnclosureId", "EnclosureId");
            return Page();
        }

        [BindProperty]
        public Closing Closing { get; set; } = default!;

        // Handle form submission and saving the new Closing
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Return to the form if validation fails
            }

            // The ClosingId is automatically generated by SQL Server since it is an identity column.
            // No need to manually set it.

            _context.Closings.Add(Closing);  // Add the new Closing to the context

            await _context.SaveChangesAsync(); // Save changes to the database

            return RedirectToPage("./Index"); // Redirect back to the list page
        }
    }
}
