using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using global::RazorLogin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Shop.Deps
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(RazorLogin.Models.ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");

            return Page();
        }

        //[BindProperty]
        //public Dependant Dependant { get; set; } = default!;

        [BindProperty]
        public Dependant Dependent { get; set; } = new Dependant();

        public async Task<IActionResult> OnPostAsync()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            // Find the employee by their email
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeEmail == user.Email);
            if (employee == null)
            {
                ModelState.AddModelError(string.Empty, "Employee not found.");
                return Page();
            }

            // Assign the EmployeeId to the dependent
            Dependent.EmployeeId = employee.EmployeeId;
            Dependent.Employee = employee; // Set the Employee navigation property

            // Generate a unique DependantId based on EmployeeId
            int employeeId = employee.EmployeeId;
            Random random = new Random();
            int randomSuffix = random.Next(10000, 99999); // Generate a random number
            Dependent.DepndantId = int.Parse($"{employeeId}{randomSuffix}"); // Concatenate employee ID and random number

            // Ensure DependantId is unique in the database
            while (await _context.Dependants.AnyAsync(d => d.DepndantId == Dependent.DepndantId))
            {
                randomSuffix = random.Next(10000, 99999);
                Dependent.DepndantId = int.Parse($"{employeeId}{randomSuffix}");
            }


            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please correct the errors and try again.");
                return Page();
            }

            // Add the dependent to the context and save changes
            _context.Dependants.Add(Dependent);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index"); // Redirect to the index page after successful creation
        }
    }


}

