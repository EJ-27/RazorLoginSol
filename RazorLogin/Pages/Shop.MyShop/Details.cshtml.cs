using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Shop.MyShop
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ZooDbContext _context;

        public DetailsModel(ZooDbContext context)
        {
            _context = context;
        }

        public FoodStore FoodStore { get; set; }
        public GiftShop GiftShop { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public int EmployeeCount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the current user's email
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return NotFound("User email not found");
            }

            // Find the employee record for the logged-in user
            var employee = await _context.Employees
                .Include(e => e.FoodStore) // Include FoodStore relationship
                .Include(e => e.Shop)      // Include GiftShop relationship
                .FirstOrDefaultAsync(e => e.EmployeeEmail == userEmail);

            if (employee == null)
            {
                return NotFound("Employee record not found");
            }

            if (employee.FoodStoreId.HasValue)
            {
                // Load the associated FoodStore and employees
                FoodStore = await _context.FoodStores
                    .Include(fs => fs.Employees)
                    .FirstOrDefaultAsync(fs => fs.FoodStoreId == employee.FoodStoreId.Value);

                Employees = FoodStore?.Employees.ToList();
                EmployeeCount = Employees?.Count ?? 0;
            }
            else if (employee.ShopId.HasValue)
            {
                // Load the associated GiftShop and employees
                GiftShop = await _context.GiftShops
                    .Include(gs => gs.Employees)
                    .FirstOrDefaultAsync(gs => gs.ShopId == employee.ShopId.Value);

                Employees = GiftShop?.Employees.ToList();
                EmployeeCount = Employees?.Count ?? 0;
            }

            return Page();
        }
    }
}

