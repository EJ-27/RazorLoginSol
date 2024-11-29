using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.GShps
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        // This will hold the list of gift shops and their manager data
        public IList<GiftShopViewModel> GiftShops { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Fetch all gift shops, including their employees (and managers if assigned)
            var giftShops = await _context.GiftShops
                .Include(g => g.Employees) // Include employees assigned to the shop
                .ThenInclude(e => e.Manager) // Include the manager associated with each employee
                .ToListAsync();

            // Map the gift shop data into the view model
            GiftShops = giftShops.Select(g => new GiftShopViewModel
            {
                ShopId = g.ShopId,
                GiftShopName = g.GiftShopName,
                GiftShopLocation = g.GiftShopLocation,
                GiftShopYearToDateSales = g.GiftShopYearToDateSales,
                GiftShopOpenTime = g.GiftShopOpenTime,
                GiftShopCloseTime = g.GiftShopCloseTime,
                // Find the manager if there's any employee with a manager assigned to the shop
                ManagerName = g.Employees
                    .Where(e => e.ShopId == g.ShopId && e.Manager != null) // Ensure manager is assigned to the employee
                    .Select(e => e.Manager.Employee.EmployeeFirstName + " " + e.Manager.Employee.EmployeeLastName)
                    .FirstOrDefault() ?? "None" // If no manager is found, return "None"
            }).ToList();
        }

    }

    // ViewModel for GiftShop with Manager information
    public class GiftShopViewModel
    {
        public int ShopId { get; set; }
        public string GiftShopName { get; set; }
        public string GiftShopLocation { get; set; }
        public int GiftShopYearToDateSales { get; set; }
        public TimeOnly GiftShopOpenTime { get; set; }
        public TimeOnly GiftShopCloseTime { get; set; }
        public string ManagerName { get; set; } // The name of the manager
    }
}
