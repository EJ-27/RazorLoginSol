using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Zook
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public IList<ZookeeperViewModel> Zookeepers { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Fetch zookeepers and their employee details
            var zookeepers = await _context.Zookeepers
                .Include(z => z.Employee) // Include the Employee details
                .ToListAsync();

            // Map the zookeepers to a view model
            Zookeepers = zookeepers.Select(z => new ZookeeperViewModel
            {
                ZookeeperId = z.ZookeeperId,
                EmployeeId = z.EmployeeId,
                EmployeeFirstName = z.Employee?.EmployeeFirstName,
                EmployeeLastName = z.Employee?.EmployeeLastName,
                AssignedDepartment = z.AssignedDepartment,
                NumAssignedCages = z.NumAssignedCages,
                TrainingRenewalDate = z.TrainingRenewalDate?.ToString("yyyy-MM-dd"),
                LastTrainingDate = z.LastTrainingDate?.ToString("yyyy-MM-dd")
            }).ToList();
        }
    }

    // ViewModel for Zookeeper with Employee details
    public class ZookeeperViewModel
    {
        public int ZookeeperId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string AssignedDepartment { get; set; }
        public short? NumAssignedCages { get; set; }
        public string TrainingRenewalDate { get; set; }
        public string LastTrainingDate { get; set; }
    }
}
