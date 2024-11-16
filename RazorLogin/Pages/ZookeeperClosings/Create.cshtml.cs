﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace RazorLogin.Pages.ZookeeperClosings
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

        [BindProperty]
        public Closing Closing { get; set; } = default!;

        public SelectList EnclosureOptions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            // Find the employee by their email
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeEmail == user.Email);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            // Find the corresponding zookeeper linked to this employee
            var zookeeper = await _context.Zookeepers
                .FirstOrDefaultAsync(z => z.EmployeeId == employee.EmployeeId);

            if (zookeeper == null)
            {
                return NotFound("Zookeeper record not found.");
            }

            // Fetch only the enclosures assigned to this zookeeper
            var enclosures = await _context.Enclosures
                .Where(e => e.ZookeeperId == zookeeper.ZookeeperId)
                .Select(e => new { e.EnclosureId, e.EnclosureName })
                .ToListAsync();

            // Set the enclosure options for the dropdown
            EnclosureOptions = new SelectList(enclosures, "EnclosureId", "EnclosureName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Generate a unique random integer for ClosingId
            var random = new Random();
            int newClosingId;
            do
            {
                newClosingId = random.Next(1, int.MaxValue);
            } while (await _context.Closings.AnyAsync(c => c.ClosingId == newClosingId));

            Closing.ClosingId = newClosingId;

            _context.Closings.Add(Closing);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

