using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RazorLogin.Pages.Manag.Sho
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public IndexModel(RazorLogin.Models.ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public GiftShop? AssignedGiftShop { get; set; }
        public FoodStore? AssignedFoodStore { get; set; }
        public IList<Employee> EmployeesUnderManager { get; set; } = new List<Employee>();

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserEmail = User.Identity.Name;

            if (string.IsNullOrEmpty(currentUserEmail))
            {
                return Page();
            }
            // Find the employee associated with the current user email
            var currentEmployee = await _context.Employees
                .Include(e => e.Shop)
                .Include(e => e.FoodStore)
                .FirstOrDefaultAsync(e => e.EmployeeEmail == currentUserEmail);

            if (currentEmployee != null)
            {
                // Check for assigned GiftShop
                if (currentEmployee.ShopId.HasValue)
                {
                    AssignedGiftShop = await _context.GiftShops
                        .Include(g => g.Employees)
                        .FirstOrDefaultAsync(g => g.ShopId == currentEmployee.ShopId);

                    // Get employees assigned to this GiftShop
                    EmployeesUnderManager = AssignedGiftShop?.Employees.ToList() ?? new List<Employee>();
                }
                // Check for assigned FoodStore
                else if (currentEmployee.FoodStoreId.HasValue)
                {
                    AssignedFoodStore = await _context.FoodStores
                        .Include(f => f.Employees)
                        .FirstOrDefaultAsync(f => f.FoodStoreId == currentEmployee.FoodStoreId);

                    // Get employees assigned to this FoodStore
                    EmployeesUnderManager = AssignedFoodStore?.Employees.ToList() ?? new List<Employee>();
                }
            }

            return Page();
        }
    }
}
