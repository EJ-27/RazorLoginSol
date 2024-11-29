using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Shop.MyShop
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        public FoodStore FoodStore { get; set; }
        public GiftShop GiftShop { get; set; }

        public async Task OnGetAsync()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return;
            }

            // Fetch employee based on the logged-in user's email
            var employee = await _context.Employees
                .Include(e => e.FoodStore)
                .Include(e => e.Shop)
                .FirstOrDefaultAsync(e => e.EmployeeEmail == userEmail);

            if (employee != null)
            {
                // Assign the correct store based on department
                if (employee.Department == "FOOD")
                {
                    FoodStore = employee.FoodStore;
                }
                else if (employee.Department == "GIFT")
                {
                    GiftShop = employee.Shop;
                }
            }
        }
    }
}
