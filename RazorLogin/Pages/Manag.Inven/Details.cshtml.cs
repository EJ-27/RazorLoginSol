using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Manag.Inven
{
    public class DetailsModel : PageModel
    {
        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Item Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Item = await _context.Items.FirstOrDefaultAsync(m => m.ItemId == id);
            return Item == null ? NotFound() : Page();
        }
    }
}

