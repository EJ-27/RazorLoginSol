using global::RazorLogin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Zoo.Myinfo
{
    [Authorize(Roles = "Zookeeper")]
    public class viewPersonalModel : PageModel
    {
        private readonly ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public viewPersonalModel(ZooDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        [BindProperty]
        public Zookeeper Zookeeper { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            Employee = await _context.Employees.FindAsync(id);
            if (Employee == null)
            {
                return NotFound();
            }

            Zookeeper = await _context.Zookeepers.FirstOrDefaultAsync(z => z.EmployeeId == Employee.EmployeeId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var employeeToUpdate = await _context.Employees.FindAsync(Employee.EmployeeId);
            if (employeeToUpdate == null)
            {
                return NotFound();
            }

            // Update the employee's personal details
            employeeToUpdate.EmployeeFirstName = Employee.EmployeeFirstName;
            employeeToUpdate.EmployeeLastName = Employee.EmployeeLastName;
            employeeToUpdate.EmployeeAddress = Employee.EmployeeAddress;
            employeeToUpdate.EmployeePhoneNumber = Employee.EmployeePhoneNumber;
            employeeToUpdate.EmployeeDob = Employee.EmployeeDob;

            if (Zookeeper != null)
            {
                var zookeeperToUpdate = await _context.Zookeepers.FirstOrDefaultAsync(z => z.EmployeeId == Employee.EmployeeId);
                if (zookeeperToUpdate != null)
                {
                    zookeeperToUpdate.AssignedDepartment = Zookeeper.AssignedDepartment;
                    zookeeperToUpdate.TrainingRenewalDate = Zookeeper.TrainingRenewalDate;
                    zookeeperToUpdate.LastTrainingDate = Zookeeper.LastTrainingDate;
                    zookeeperToUpdate.NumAssignedCages = Zookeeper.NumAssignedCages;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
