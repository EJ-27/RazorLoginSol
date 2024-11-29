using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Mana
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IList<ManagerViewModel> Managers { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Load managers with their employee and store data (FoodStore and GiftShop)
            var managers = await _context.Managers
                .Include(m => m.Employee)
                .ThenInclude(e => e.FoodStore) // Include the FoodStore related to the employee
                .Include(m => m.Employee)
                .ThenInclude(e => e.Shop) // Include the GiftShop related to the employee
                .ToListAsync();

            // Map the managers to a view model with the assigned store info (ID and Name)
            Managers = managers.Select(m => new ManagerViewModel
            {
                ManagerId = m.ManagerId,
                EmployeeId = m.EmployeeId,
                Department = m.Department,
                ManagerEmploymentDate = m.ManagerEmploymentDate,
                EmployeeFirstName = m.Employee.EmployeeFirstName ?? "N/A", // Get the employee's first name
                EmployeeLastName = m.Employee.EmployeeLastName ?? "N/A",  // Get the employee's last name

                // Get the formatted store info (ID - Name)
                StoreInfo = GetStoreInfo(m.Employee) // Assign formatted store info
            }).ToList();
        }

        // Helper method to get the store info (ID and Name)
        private string GetStoreInfo(Employee employee)
        {
            if (employee.FoodStore != null)
            {
                // Format store info for FoodStore
                return $"{employee.FoodStore.FoodStoreId} - {employee.FoodStore.FoodStoreName}";
            }
            else if (employee.Shop != null)
            {
                // Format store info for GiftShop
                return $"{employee.Shop.ShopId} - {employee.Shop.GiftShopName}";
            }
            else
            {
                return "No store assigned"; // If no store is assigned
            }
        }

    }

    // ViewModel to display manager details with assigned store
    public class ManagerViewModel
    {
        public int ManagerId { get; set; }
        public int EmployeeId { get; set; }
        public string Department { get; set; }
        public DateOnly? ManagerEmploymentDate { get; set; }

        // New property to hold formatted store info (ID and Name)
        public string StoreInfo { get; set; } = null!;

        // Add these properties to hold the employee's name
        public string EmployeeFirstName { get; set; } = null!;
        public string EmployeeLastName { get; set; } = null!;
    }


}




/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Mana
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IList<RazorLogin.Models.Manager> Manager { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Manager = await _context.Managers
                .Include(m => m.Employee).ToListAsync();
        }
    }
}*/
