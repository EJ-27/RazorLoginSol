using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RazorLogin.Models;

namespace RazorLogin.Pages.TicketsPage
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(RazorLogin.Models.ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Ticket Ticket { get; set; } = new Ticket();

        public Dictionary<string, int> TicketTypes { get; } = new Dictionary<string, int>
        {
            { "Adult", 25 },
            { "Child", 10 },
            { "Senior", 10 },
            { "Veteran", 7 }
        };

        [BindProperty]
        public string SelectedTicketType { get; set; } = string.Empty;

        public string MembershipType { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var userEmail = user?.Email;

            if (userEmail != null)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerEmail == userEmail);
                MembershipType = customer?.MembershipType;
            }

            // If a ticket type was selected previously, retain the selected value
            if (!string.IsNullOrEmpty(SelectedTicketType))
            {
                Ticket.TicketType = SelectedTicketType;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Get the logged-in user's email
            var user = await _userManager.GetUserAsync(User);
            var userEmail = user?.Email;

            if (userEmail == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            // Retrieve the customer based on the logged-in user's email
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerEmail == userEmail);
            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "Customer not found.");
                return Page();
            }

            // Assign selected ticket type to the Ticket model
            Ticket.TicketType = SelectedTicketType;

            // Set the ticket price based on selected ticket type
            if (TicketTypes.TryGetValue(SelectedTicketType, out var basePrice))
            {
                Ticket.TicketPrice = basePrice;

                // Apply membership discount
                switch (customer.MembershipType)
                {
                    case "FAMILY TIER":
                        Ticket.TicketPrice -= 2;
                        break;
                    case "VIP TIER":
                        Ticket.TicketPrice -= 3;
                        break;
                }
            }
            else
            {
                ModelState.AddModelError("Ticket.TicketType", "Invalid ticket type selected.");
                return Page();
            }

            // Generate random IDs for Ticket and Purchase
            Ticket.TicketId = GenerateRandomId();
            Ticket.TicketPurchaseDate = DateOnly.FromDateTime(DateTime.Now);

            // Get current purchase time
            var purchaseTime = TimeOnly.FromDateTime(DateTime.Now);

            var purchase = new Purchase
            {
                PurchaseId = GenerateRandomId(),
                CustomerId = customer.CustomerId,
                PurchaseDate = Ticket.TicketPurchaseDate,
                PurchaseTime = purchaseTime, 
                TotalPurchasesPrice = Ticket.TicketPrice,
                NumItems = 1,
                ItemName = "Ticket" 
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            // Link the Purchase to the Ticket
            Ticket.Purchase = purchase;
            _context.Tickets.Add(Ticket);

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your ticket has been successfully purchased!";
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error saving ticket: {ex.Message}");
                ModelState.AddModelError("Ticket", "An error occurred while saving the ticket. Please try again.");
                return Page();
            }

            return Page();
        }

        private int GenerateRandomId()
        {
            var random = new Random();
            int newId;

            do
            {
                newId = random.Next(1000, 9999);
            }
            while (_context.Tickets.Any(t => t.TicketId == newId) ||
                   _context.Purchases.Any(p => p.PurchaseId == newId)); // Ensure no conflict with existing IDs

            return newId;
        }
    }
}
