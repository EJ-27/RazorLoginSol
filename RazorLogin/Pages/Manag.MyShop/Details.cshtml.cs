using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manag.MyShop
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ZooDbContext _context;

        public DetailsModel(ZooDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FoodStore FoodStore { get; set; }

        [BindProperty]
        public GiftShop GiftShop { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
        public int EmployeeCount { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return NotFound("User email not found.");
            }

            var employee = await _context.Employees
                .Include(e => e.FoodStore)
                .Include(e => e.Shop)
                .FirstOrDefaultAsync(e => e.EmployeeEmail == userEmail);

            if (employee == null)
            {
                return NotFound("Employee record not found.");
            }

            if (employee.FoodStoreId.HasValue)
            {
                FoodStore = await _context.FoodStores
                    .Include(fs => fs.Employees)
                    .FirstOrDefaultAsync(fs => fs.FoodStoreId == employee.FoodStoreId.Value);

                Employees = FoodStore?.Employees.ToList() ?? new List<Employee>();
                EmployeeCount = Employees.Count;
            }
            else if (employee.ShopId.HasValue)
            {
                GiftShop = await _context.GiftShops
                    .Include(gs => gs.Employees)
                    .FirstOrDefaultAsync(gs => gs.ShopId == employee.ShopId.Value);

                Employees = GiftShop?.Employees.ToList() ?? new List<Employee>();
                EmployeeCount = Employees.Count;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (FoodStore != null)
            {
                var existingFoodStore = await _context.FoodStores.FindAsync(FoodStore.FoodStoreId);
                if (existingFoodStore != null)
                {
                    existingFoodStore.FoodStoreName = FoodStore.FoodStoreName;
                    existingFoodStore.FoodStoreLocation = FoodStore.FoodStoreLocation;
                    existingFoodStore.FoodStoreOpenTime = FoodStore.FoodStoreOpenTime;
                    existingFoodStore.FoodStoreCloseTime = FoodStore.FoodStoreCloseTime;
                    existingFoodStore.IsDeleted = FoodStore.IsDeleted;
                }
            }
            else if (GiftShop != null)
            {
                var existingGiftShop = await _context.GiftShops.FindAsync(GiftShop.ShopId);
                if (existingGiftShop != null)
                {
                    existingGiftShop.GiftShopName = GiftShop.GiftShopName;
                    existingGiftShop.GiftShopLocation = GiftShop.GiftShopLocation;
                    existingGiftShop.GiftShopOpenTime = GiftShop.GiftShopOpenTime;
                    existingGiftShop.GiftShopCloseTime = GiftShop.GiftShopCloseTime;
                    existingGiftShop.IsDeleted = GiftShop.IsDeleted;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred while saving: {ex.Message}");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}