using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RazorLogin.Models;

namespace RazorLogin.Pages.CustomerShop
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(RazorLogin.Models.ZooDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<IndexModel> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public IList<Item> Item { get; set; } = default!;
        public string SuccessMessage { get; set; } = string.Empty; 

        public async Task OnGetAsync()
        {
            // Retrieve items only from ShopId/StoreId 2
            Item = await _context.Items.Where(i => i.ShopId == 2).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync(int itemId, int quantity)
        {
            // Get the logged-in user's email
            var userEmail = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("User email is null or empty.");
                return Page();
            }

            // Retrieve customer by email
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerEmail == userEmail);
            if (customer == null)
            {
                _logger.LogWarning($"No customer found for email: {userEmail}");
                return Page();
            }

            // Find the item being purchased
            var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == itemId && i.ShopId == 2);
            if (item == null || item.ItemCount < quantity)
            {
                ModelState.AddModelError("", "Insufficient stock or item not found.");
                return Page();
            }

            // Calculate the total price of the purchase
            var totalPurchasePrice = item.ItemPrice * quantity;

            // Update the stock of the item
            item.ItemCount -= quantity;
            _context.Items.Update(item);

            // Generate a unique PurchaseId using a random number
            Random random = new Random();
            int purchaseId;
            do
            {
                purchaseId = random.Next(10000, 99999);
            } while (await _context.Purchases.AnyAsync(p => p.PurchaseId == purchaseId));

            // Create a new purchase record
            var purchase = new Purchase
            {
                PurchaseId = purchaseId,
                CustomerId = customer.CustomerId,
                PurchaseDate = DateOnly.FromDateTime(DateTime.Now),
                PurchaseTime = TimeOnly.FromDateTime(DateTime.Now),
                NumItems = quantity,
                StoreId = item.ShopId,
                TotalPurchasesPrice = totalPurchasePrice,
                ItemName = item.ItemName 
            };

            // Save the new purchase
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            // Store the success message in TempData to display after redirect
            TempData["SuccessMessage"] = "Purchase successful! Thank you for your order.";

            return RedirectToPage("./Index");
        }
    }
}
