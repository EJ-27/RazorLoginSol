using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Cust
{
    public class DeleteModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public DeleteModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }
            else
            {
                Customer = customer;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ModelState.Remove("Customer.CustomerEmail");

            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                // Check if the customer is related to any other entities (e.g., orders, etc.)
                // You may need to check for foreign key relationships before deleting

                // Remove the customer from the database
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                // If no error, redirect to Index
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException dbEx)
            {
                // Handle specific database errors (e.g., foreign key violations)
                ModelState.AddModelError(string.Empty, "Cannot delete the customer because they are linked to Purchases and/or Ticket Purcases. " + dbEx.Message);
                return Page(); // Return to the same page with the error
            }
            catch (Exception ex)
            {
                // Handle any other general errors
                ModelState.AddModelError(string.Empty, "An unexpected error occurred: " + ex.Message);
                return Page(); // Return to the same page with the error
            }
        }
    }
}
