using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.CustomerPage
{
    public class EditModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public EditModel(RazorLogin.Models.ZooDbContext context)
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

            // Load the customer data into the bindable property
            Customer = customer;
            return Page();
        }

        // Modify only specific fields (First Name, Last Name, DOB, Email, and Address)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Retrieve the customer record from the database
            var customerInDb = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == Customer.CustomerId);
            
            if (customerInDb == null)
            {
                return NotFound();
            }

            // Update only the allowed fields
            customerInDb.CustomerFirstName = Customer.CustomerFirstName;
            customerInDb.CustomerLastName = Customer.CustomerLastName;
            customerInDb.CustomerAddress = Customer.CustomerAddress;
            customerInDb.CustomerEmail = Customer.CustomerEmail;
            customerInDb.CustomerDob = Customer.CustomerDob;

            // The fields below are NOT being updated
            // customerInDb.MembershipStartDate; (unchanged)
            // customerInDb.MembershipEndDate;   (unchanged)
            // customerInDb.MembershipType;      (unchanged)
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(Customer.CustomerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/CustomerPage/Index");
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
