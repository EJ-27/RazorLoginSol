using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Mana
{
    public class MoveEmpModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public MoveEmpModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        // View data for drop-downs
        public SelectList FoodStores { get; set; } = default!;
        public SelectList GiftShops { get; set; } = default!;
        public SelectList Supervisors { get; set; } = default!;

        // On GET request, fetch employee details and populate drop-down lists
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Fetch the filtered list of Gift Shops and Food Stores with managers
            var giftShopsWithManagers = _context.Managers
                .Where(mgr => mgr.EmployeeId != null)
                .Join(_context.Employees,
                    mgr => mgr.EmployeeId,
                    emp => emp.EmployeeId,
                    (mgr, emp) => new { emp.ShopId })
                .Where(x => x.ShopId.HasValue)
                .Select(x => x.ShopId.Value)
                .Distinct()
                .ToList();

            var foodStoresWithManagers = _context.Managers
                .Where(mgr => mgr.EmployeeId != null)
                .Join(_context.Employees,
                    mgr => mgr.EmployeeId,
                    emp => emp.EmployeeId,
                    (mgr, emp) => new { emp.FoodStoreId })
                .Where(x => x.FoodStoreId.HasValue)
                .Select(x => x.FoodStoreId.Value)
                .Distinct()
                .ToList();

            // Fetch Gift Shops with managers
            var giftShopsWithManagersSelectList = _context.GiftShops
                .Where(shop => giftShopsWithManagers.Contains(shop.ShopId))
                .Select(shop => new SelectListItem
                {
                    Value = shop.ShopId.ToString(),
                    Text = shop.GiftShopName
                })
                .ToList();

            // Fetch Food Stores with managers
            var foodStoresWithManagersSelectList = _context.FoodStores
                .Where(store => foodStoresWithManagers.Contains(store.FoodStoreId))
                .Select(store => new SelectListItem
                {
                    Value = store.FoodStoreId.ToString(),
                    Text = store.FoodStoreName
                })
                .ToList();

            // Populate ViewData with lists for dropdowns
            ViewData["GiftShopsWithManagers"] = giftShopsWithManagersSelectList;
            ViewData["FoodStoresWithManagers"] = foodStoresWithManagersSelectList;

            // If ID is null or invalid, return NotFound
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            Employee = employee;
            return Page();
        }

        // On POST request, handle employee update logic
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // If the model is invalid, reload the page with the current employee data
                FoodStores = new SelectList(_context.FoodStores, "FoodStoreId", "FoodStoreName");
                GiftShops = new SelectList(_context.GiftShops, "ShopId", "GiftShopName");
                Supervisors = new SelectList(_context.Managers, "ManagerId", "Department");
                return Page();
            }

            // Handle the logic of updating food store or gift shop based on the manager selection
            if (Employee.FoodStoreId != null)
            {
                // If a food store is selected, nullify the GiftShopId
                Employee.ShopId = null;
            }
            else if (Employee.ShopId != null)
            {
                // If a gift shop is selected, nullify the FoodStoreId
                Employee.FoodStoreId = null;
            }

            // Mark the employee as modified and update the database
            _context.Attach(Employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(Employee.EmployeeId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Redirect to the employee list after saving the changes
            return RedirectToPage("./EmpList", new { managerId = Employee.SupervisorId });
        }

        // Check if the employee exists in the database
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
