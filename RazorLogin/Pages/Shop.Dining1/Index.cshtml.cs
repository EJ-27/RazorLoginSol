using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.Dining1
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        // This will hold the list of food stores and their manager data
        public IList<FoodStoreViewModel> FoodStores { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Fetch all food stores, including their employees (and managers if assigned)
            var foodStores = await _context.FoodStores
                .Include(f => f.Employees) // Include employees assigned to the food store
                .ThenInclude(e => e.Manager) // Include the manager associated with each employee
                .ToListAsync();

            // Map the food store data into the view model
            FoodStores = foodStores.Select(f => new FoodStoreViewModel
            {
                FoodStoreId = f.FoodStoreId,
                FoodStoreName = f.FoodStoreName,
                FoodStoreLocation = f.FoodStoreLocation,
                FoodStoreYearToDateSales = f.FoodStoreYearToDateSales,
                FoodStoreOpenTime = f.FoodStoreOpenTime,
                FoodStoreCloseTime = f.FoodStoreCloseTime,
                // Find the manager if there's any employee with a manager assigned to the store
                ManagerName = f.Employees
                    .Where(e => e.FoodStoreId == f.FoodStoreId && e.Manager != null) // Ensure manager is assigned to the employee
                    .Select(e => e.Manager.Employee.EmployeeFirstName + " " + e.Manager.Employee.EmployeeLastName)
                    .FirstOrDefault() ?? "None" // If no manager is found, return "None"
            }).ToList();
        }

    }

    // ViewModel for FoodStore with Manager information
    public class FoodStoreViewModel
    {
        public int FoodStoreId { get; set; }
        public string FoodStoreName { get; set; }
        public string FoodStoreLocation { get; set; }
        public int FoodStoreYearToDateSales { get; set; }
        public TimeOnly? FoodStoreOpenTime { get; set; } // Nullable TimeOnly
        public TimeOnly? FoodStoreCloseTime { get; set; } // Nullable TimeOnly
        public string ManagerName { get; set; } // The name of the manager
    }
}
