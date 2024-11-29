using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manag.Sho
{
    public class EditModel : PageModel
    {
        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public GiftShop GiftShop { get; set; }

        [BindProperty]
        public FoodStore FoodStore { get; set; }

        public bool IsGiftShop { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserEmail = currentUser?.Email;

            if (string.IsNullOrEmpty(currentUserEmail))
                return RedirectToPage("/Account/Login");

            var employee = await _context.Employees
                .Include(e => e.Shop) // Assuming you have navigation properties
                .Include(e => e.FoodStore)
                .FirstOrDefaultAsync(e => e.EmployeeEmail == currentUserEmail);

            if (employee == null)
                return NotFound("No assigned store found.");

            if (employee.ShopId != null)
            {
                GiftShop = await _context.GiftShops.FindAsync(employee.ShopId);
                GiftShop.ShopId = employee.ShopId;
                IsGiftShop = true;
            }
            else if (employee.FoodStoreId != null)
            {
                FoodStore = await _context.FoodStores.FindAsync(employee.FoodStoreId);
                FoodStore.FoodStoreId = employee.FoodStoreId;
                IsGiftShop = false;
            }
            else
            {
                return NotFound("Store not found.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Log ModelState errors
                return Page();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserEmail = currentUser?.Email;

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeEmail == currentUserEmail);

            if (employee == null)
                return NotFound("Employee not found.");

            try
            {
                if (IsGiftShop && employee.ShopId == GiftShop.ShopId)
                {
                    _context.Entry(GiftShop).State = EntityState.Modified;
                }
                else if (!IsGiftShop && employee.FoodStoreId == FoodStore.FoodStoreId)
                {
                    _context.Entry(FoodStore).State = EntityState.Modified;
                }
                else
                {
                    return Forbid(); // Unauthorized access
                }

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, $"Unable to save changes: {ex.Message}");
                return Page();
            }
        }
    }
}
