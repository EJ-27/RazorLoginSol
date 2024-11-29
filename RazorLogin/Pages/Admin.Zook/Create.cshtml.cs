using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Zook
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public CreateModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            return Page();
        }

        [BindProperty]
        public Zookeeper Zookeeper { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
       public async Task<IActionResult> OnPostAsync()
       {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Generate a unique zookid based on EmployeeId
            int employeeId = Zookeeper.EmployeeId;
            Random random = new Random();
            int randomSuffix = random.Next(10000, 99999); // Generate a random number
            Zookeeper.ZookeeperId = int.Parse($"{employeeId}{randomSuffix}"); // Concatenate employee ID and random number

            // Ensure zookid is unique in the database
            while (await _context.Dependants.AnyAsync(d => d.DepndantId == Zookeeper.ZookeeperId))
            {
                randomSuffix = random.Next(10000, 99999);
                Zookeeper.ZookeeperId = int.Parse($"{employeeId}{randomSuffix}");
            }

            _context.Zookeepers.Add(Zookeeper);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
       }

    
}
}
