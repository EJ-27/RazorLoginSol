/*using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Admin.Reports2
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }


        // Properties for user input
        [BindProperty]
        public DateTime? StartDate { get; set; }
        [BindProperty]
        public DateTime? EndDate { get; set; }
        [BindProperty]
        public bool SearchAll { get; set; } = true; // Default to search all employees

        public ReportData Report { get; set; }

        public async Task OnGetAsync()
        {
            Report = new ReportData();

            var query = _context.Set<Employee>().AsQueryable();


            var employees = await query.ToListAsync(); // Retrieve data into memory

            // 1. Employee Stats

            var degree = "HIGH SCHOOL DIPLOMA";
            // Average Age
            var EAvgAgeHS = _context.Database.SqlQueryRaw<decimal>($"SELECT AVG (1.0 * CASE WHEN Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(Employee_DOB) ELSE 0 END) AS Value FROM dbo.Employee WHERE Degree = @p0",  degree).FirstOrDefault();

            // Minimum Age
            var EMinAgeHS = _context.Database.SqlQueryRaw<int>($"SELECT MIN (1.0 * CASE WHEN Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(Employee_DOB) ELSE 0 END) AS Value FROM dbo.Employee WHERE Degree = @p", degree).FirstOrDefault();

            // Maximum Age
            var EMaxAgeHS = _context.Database.SqlQueryRaw<int>($"SELECT MAX (1.0 * CASE WHEN Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(Employee_DOB) ELSE 0 END) AS Value FROM dbo.Employee WHERE Degree = @p0", degree).FirstOrDefault();

            // Count
            var ECountHS = _context.Database.SqlQueryRaw<int>($"SELECT COUNT (1.0 * CASE WHEN Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(Employee_DOB) ELSE 0 END) AS Value FROM dbo.Employee WHERE Degree = @p0", degree).FirstOrDefault();
           
            Report.EmployeeAvgAge = (decimal)EAvgAgeHS;
            Report.EmployeeMaxAge = (double)EMaxAgeHS;
            Report.EmployeeMinAge = (double)EMinAgeHS;
           // Report.EmployeeCountHS = ECountHS;



     

            // 2. Manager Stats
            var managerStats = await _context.Set<Manager>()
                .Join(_context.Set<Employee>(), m => m.EmployeeId, e => e.EmployeeId, (m, e) => new { e, m })
                .ToListAsync(); // Retrieve data into memory

            var managerGroupedStats = managerStats
                .GroupBy(em => em.e.Degree ?? "null")
                .Select(g => new StatDto
                {
                    Degree = g.Key,
                    AverageAge = g.Where(em => em.e.EmployeeDob.HasValue)
                                  .Average(em => DateTime.Now.Year - em.e.EmployeeDob.Value.Year -
                                                 (DateTime.Now.DayOfYear < em.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MinAge = g.Where(em => em.e.EmployeeDob.HasValue)
                               .Min(em => DateTime.Now.Year - em.e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < em.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MaxAge = g.Where(em => em.e.EmployeeDob.HasValue)
                               .Max(em => DateTime.Now.Year - em.e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < em.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    Count = g.Count()
                })
                .ToList();

            if (!managerGroupedStats.Any())
            {
                managerGroupedStats.Add(new StatDto { Degree = "No Data", AverageAge = 0, MinAge = 0, MaxAge = 0, Count = 0 });
            }

          //  Report.ManagerStats = managerGroupedStats;

            // 3. Zookeeper Stats
            var zookeeperStats = await _context.Set<Zookeeper>()
                .Join(_context.Set<Employee>(), z => z.EmployeeId, e => e.EmployeeId, (z, e) => new { e, z })
                .ToListAsync(); // Retrieve data into memory

            var zookeeperGroupedStats = zookeeperStats
                .GroupBy(ze => ze.e.Degree ?? "null")
                .Select(g => new StatDto
                {
                    Degree = g.Key,
                    AverageAge = g.Where(ze => ze.e.EmployeeDob.HasValue)
                                  .Average(ze => DateTime.Now.Year - ze.e.EmployeeDob.Value.Year -
                                                 (DateTime.Now.DayOfYear < ze.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MinAge = g.Where(ze => ze.e.EmployeeDob.HasValue)
                               .Min(ze => DateTime.Now.Year - ze.e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < ze.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MaxAge = g.Where(ze => ze.e.EmployeeDob.HasValue)
                               .Max(ze => DateTime.Now.Year - ze.e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < ze.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    Count = g.Count()
                })
                .ToList();

            if (!zookeeperGroupedStats.Any())
            {
                zookeeperGroupedStats.Add(new StatDto { Degree = "No Data", AverageAge = 0, MinAge = 0, MaxAge = 0, Count = 0 });
            }

            Report.ZookeeperStats = zookeeperGroupedStats;

            // Prepare data for average ages overview
            Report.AverageAgesOverview = new double[]
            {
                employeeStats.Average(s => s.AverageAge),
                managerGroupedStats.Average(s => s.AverageAge),
                zookeeperGroupedStats.Average(s => s.AverageAge)
            };

            // Prepare data for degree counts overview
            Report.DegreeCountsOverview = new Dictionary<string, int>();
            foreach (var stat in employeeStats.Concat(managerGroupedStats).Concat(zookeeperGroupedStats))
            {
                if (!string.IsNullOrEmpty(stat.Degree)) // Check if Degree is not null or empty
                {
                    if (Report.DegreeCountsOverview.ContainsKey(stat.Degree))
                    {
                        Report.DegreeCountsOverview[stat.Degree] += stat.Count;
                    }
                    else
                    {
                        Report.DegreeCountsOverview[stat.Degree] = stat.Count;
                    }
                }
            }
        }
    }

  

    public class StatDto
    {
        public string Degree { get; set; }
        public double AverageAge { get; set; }
        public double MinAge { get; set; }
        public double MaxAge { get; set; }
        public int Count { get; set; }
    }

    public class ReportData
    {
        public decimal TotalDifference { get; set; }
        public int totalEmployeeSalary { get; set; }
        public decimal EmployeeAvgAge { get; set; }
        public double EmployeeMinAge { get; set; }
        public double EmployeeMaxAge { get; set; }
        public decimal EmployeeAvgSalary { get; set; }
        public double EmployeeMinSalary { get; set; }
        public double EmployeeMaxSalary { get; set; }
        public int Tier1Income { get; set; }
        public int Tier2Income { get; set; }
        public int Tier3Income { get; set; }
        public decimal ManagerAvgAge { get; set; }
        public double ManagerMinAge { get; set; }
        public double ManagerMaxAge { get; set; }
        public decimal ManagerAvgSalary { get; set; }
        public double ManagerMinSalary { get; set; }
        public double ManagerMaxSalary { get; set; }
        public decimal ZookeeperAvgAge { get; set; }
        public double ZookeeperMinAge { get; set; }
        public double ZookeeperMaxAge { get; set; }
        public decimal ZookeeperAvgSalary { get; set; }
        public double ZookeeperMinSalary { get; set; }
        public double ZookeeperMaxSalary { get; set; }
    }
}





/*
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Admin.Reports2
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        // Properties for user input
        [BindProperty]
        public DateTime? StartDate { get; set; }
        [BindProperty]
        public DateTime? EndDate { get; set; }
        [BindProperty]
        public bool SearchAll { get; set; } = true; // Default to search all employees

        public ReportData Report { get; set; }

        public async Task OnGetAsync()
        {
            Report = new ReportData();

            var query = _context.Set<Employee>().AsQueryable();

            // Filter by date if StartDate and EndDate are provided
            if (StartDate.HasValue && EndDate.HasValue)
            {
                query = query.Where(e => e.DateOfEmployment.HasValue
                    && e.DateOfEmployment.Value.ToDateTime(TimeOnly.MinValue) >= StartDate.Value
                    && e.DateOfEmployment.Value.ToDateTime(TimeOnly.MinValue) <= EndDate.Value);
            }

            var employees = await query.ToListAsync(); // Retrieve data into memory

            // 1. Employee Stats
            var employeeStats = employees
                .GroupBy(e => e.Degree ?? "null")
                .Select(g => new StatDto
                {
                    Degree = g.Key,
                    AverageAge = g.Where(e => e.EmployeeDob.HasValue)
                                  .Average(e => DateTime.Now.Year - e.EmployeeDob.Value.Year -
                                                (DateTime.Now.DayOfYear < e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MinAge = g.Where(e => e.EmployeeDob.HasValue)
                               .Min(e => DateTime.Now.Year - e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MaxAge = g.Where(e => e.EmployeeDob.HasValue)
                               .Max(e => DateTime.Now.Year - e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    Count = g.Count()
                })
                .ToList();

            // If no stats found, set a message
            if (!employeeStats.Any())
            {
                employeeStats.Add(new StatDto { Degree = "No Data", AverageAge = 0, MinAge = 0, MaxAge = 0, Count = 0 });
            }

            Report.EmployeeStats = employeeStats;

            // 2. Manager Stats
            var managerStats = await _context.Set<Manager>()
                .Join(_context.Set<Employee>(), m => m.EmployeeId, e => e.EmployeeId, (m, e) => new { e, m })
                .ToListAsync(); // Retrieve data into memory

            var managerGroupedStats = managerStats
                .GroupBy(em => em.e.Degree ?? "null")
                .Select(g => new StatDto
                {
                    Degree = g.Key,
                    AverageAge = g.Where(em => em.e.EmployeeDob.HasValue)
                                  .Average(em => DateTime.Now.Year - em.e.EmployeeDob.Value.Year -
                                                 (DateTime.Now.DayOfYear < em.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MinAge = g.Where(em => em.e.EmployeeDob.HasValue)
                               .Min(em => DateTime.Now.Year - em.e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < em.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MaxAge = g.Where(em => em.e.EmployeeDob.HasValue)
                               .Max(em => DateTime.Now.Year - em.e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < em.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    Count = g.Count()
                })
                .ToList();

            if (!managerGroupedStats.Any())
            {
                managerGroupedStats.Add(new StatDto { Degree = "No Data", AverageAge = 0, MinAge = 0, MaxAge = 0, Count = 0 });
            }

            Report.ManagerStats = managerGroupedStats;

            // 3. Zookeeper Stats
            var zookeeperStats = await _context.Set<Zookeeper>()
                .Join(_context.Set<Employee>(), z => z.EmployeeId, e => e.EmployeeId, (z, e) => new { e, z })
                .ToListAsync(); // Retrieve data into memory

            var zookeeperGroupedStats = zookeeperStats
                .GroupBy(ze => ze.e.Degree ?? "null")
                .Select(g => new StatDto
                {
                    Degree = g.Key,
                    AverageAge = g.Where(ze => ze.e.EmployeeDob.HasValue)
                                  .Average(ze => DateTime.Now.Year - ze.e.EmployeeDob.Value.Year -
                                                 (DateTime.Now.DayOfYear < ze.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MinAge = g.Where(ze => ze.e.EmployeeDob.HasValue)
                               .Min(ze => DateTime.Now.Year - ze.e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < ze.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    MaxAge = g.Where(ze => ze.e.EmployeeDob.HasValue)
                               .Max(ze => DateTime.Now.Year - ze.e.EmployeeDob.Value.Year -
                                         (DateTime.Now.DayOfYear < ze.e.EmployeeDob.Value.DayOfYear ? 1 : 0)),
                    Count = g.Count()
                })
                .ToList();

            if (!zookeeperGroupedStats.Any())
            {
                zookeeperGroupedStats.Add(new StatDto { Degree = "No Data", AverageAge = 0, MinAge = 0, MaxAge = 0, Count = 0 });
            }

            Report.ZookeeperStats = zookeeperGroupedStats;

            // Prepare data for average ages overview
            Report.AverageAgesOverview = new double[]
            {
                employeeStats.Average(s => s.AverageAge),
                managerGroupedStats.Average(s => s.AverageAge),
                zookeeperGroupedStats.Average(s => s.AverageAge)
            };

            // Prepare data for degree counts overview
            Report.DegreeCountsOverview = new Dictionary<string, int>();
            foreach (var stat in employeeStats.Concat(managerGroupedStats).Concat(zookeeperGroupedStats))
            {
                if (!string.IsNullOrEmpty(stat.Degree)) // Check if Degree is not null or empty
                {
                    if (Report.DegreeCountsOverview.ContainsKey(stat.Degree))
                    {
                        Report.DegreeCountsOverview[stat.Degree] += stat.Count;
                    }
                    else
                    {
                        Report.DegreeCountsOverview[stat.Degree] = stat.Count;
                    }
                }
            }
        }
    }

    public class ReportData
    {
        public List<StatDto> EmployeeStats { get; set; } = new List<StatDto>();
        public List<StatDto> ManagerStats { get; set; } = new List<StatDto>();
        public List<StatDto> ZookeeperStats { get; set; } = new List<StatDto>();

        // New properties for graphs
        public double[] AverageAgesOverview { get; set; }
        public Dictionary<string, int> DegreeCountsOverview { get; set; } = new Dictionary<string, int>();
    }

    public class StatDto
    {
        public string Degree { get; set; }
        public double AverageAge { get; set; }
        public double MinAge { get; set; }
        public double MaxAge { get; set; }
        public int Count { get; set; }
    }
}
*/




/*cshtml

@page
@model RazorLogin.Pages.Admin.Reports2.IndexModel
@{
    ViewData["Title"] = "Reports";
}

<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

<h2 class="mt-4">Employee Reports</h2>

<div class="container mt-4 mb-5">
    <!-- Added mb-5 for bottom margin -->
    <div class="mt-4">
        <h3>Employee Statistics</h3>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Degree</th>
                    <th>Average Age</th>
                    <th>Min Age</th>
                    <th>Max Age</th>
                    <th>Count</th>
                </tr>
            </thead>
            <tbody>
                @if (Model.Report.EmployeeStats.Any())
                {
                    foreach (var stat in Model.Report.EmployeeStats)
                    {
                        <tr>
                            <td>@stat.Degree</td>
                            <td>@stat.AverageAge</td>
                            <td>@stat.MinAge</td>
                            <td>@stat.MaxAge</td>
                            <td>@stat.Count</td>
                        </tr>
                    }
                }
                else
                {
                    <tr>
                        <td colspan="5">No data available for Employees.</td>
                    </tr>
                }
            </tbody>
        </table>

        <h3>Manager Statistics</h3>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Degree</th>
                    <th>Average Age</th>
                    <th>Min Age</th>
                    <th>Max Age</th>
                    <th>Count</th>
                </tr>
            </thead>
            <tbody>
                @if (Model.Report.ManagerStats.Any())
                {
                    foreach (var stat in Model.Report.ManagerStats)
                    {
                        <tr>
                            <td>@stat.Degree</td>
                            <td>@stat.AverageAge</td>
                            <td>@stat.MinAge</td>
                            <td>@stat.MaxAge</td>
                            <td>@stat.Count</td>
                        </tr>
                    }
                }
                else
                {
                    <tr>
                        <td colspan="5">No data available for Managers.</td>
                    </tr>
                }
            </tbody>
        </table>

        <h3>Zookeeper Statistics</h3>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Degree</th>
                    <th>Average Age</th>
                    <th>Min Age</th>
                    <th>Max Age</th>
                    <th>Count</th>
                </tr>
            </thead>
            <tbody>
                @if (Model.Report.ZookeeperStats.Any())
                {
                    foreach (var stat in Model.Report.ZookeeperStats)
                    {
                        <tr>
                            <td>@stat.Degree</td>
                            <td>@stat.AverageAge</td>
                            <td>@stat.MinAge</td>
                            <td>@stat.MaxAge</td>
                            <td>@stat.Count</td>
                        </tr>
                    }
                }
                else
                {
                    <tr>
                        <td colspan="5">No data available for Zookeepers.</td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div> 

<div class="container mt-4 mb-5">
    <!-- Added mb-5 for bottom margin -->
    <a class="btn btn-primary" asp-page="/Admin/Reports2/Index">Back to Reports</a>
</div>
<script>
    // Data for average age chart
    const averageAgeData = {
        labels: ['Employees', 'Managers', 'Zookeepers'],
        datasets: [{
            label: 'Average Age',
            data: [
    @Model.Report.EmployeeStats.Any() ? Model.Report.EmployeeStats.Average(s => s.AverageAge) : 0,
    @Model.Report.ManagerStats.Any() ? Model.Report.ManagerStats.Average(s => s.AverageAge) : 0,
    @Model.Report.ZookeeperStats.Any() ? Model.Report.ZookeeperStats.Average(s => s.AverageAge) : 0
            ],
            backgroundColor: ['rgba(75, 192, 192, 0.6)', 'rgba(255, 159, 64, 0.6)', 'rgba(153, 102, 255, 0.6)']
        }]
    };

    // Data for degree count chart
    const degreeCountLabels = @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Model.Report.DegreeCountsOverview.Keys.ToList()));
    const degreeCountData = {
        labels: degreeCountLabels,
        datasets: [{
            label: 'Count of Degrees',
            data: @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Model.Report.DegreeCountsOverview.Values.ToList())),
            backgroundColor: 'rgba(75, 192, 192, 0.6)'
        }]
    };

    // Chart options
    const options = {
        scales: {
            y: {
                beginAtZero: true
            }
        }
    };

    // Render average age chart
    const ctx1 = document.getElementById('averageAgeChart').getContext('2d');
    new Chart(ctx1, {
        type: 'bar',
        data: averageAgeData,
        options: options
    });

    // Render degree count chart
    const ctx2 = document.getElementById('degreeCountChart').getContext('2d');
    new Chart(ctx2, {
        type: 'bar',
        data: degreeCountData,
        options: options
    });
</script>
*/