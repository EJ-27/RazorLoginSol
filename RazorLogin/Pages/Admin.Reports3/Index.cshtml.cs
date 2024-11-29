using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using RazorLogin.Pages.Admin.Reports;

namespace RazorLogin.Pages.Admin.Reports3
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

            var degrees = new[]
           {
                "HIGH SCHOOL DIPLOMA",
                "MASTERS DEGREE",
                "BACHELORS DEGREE",
                "DOCTORAL DEGREE",
                "ASSOCIATE DEGREE",
                null // Include NULL if you want to track it as well
            };

            foreach (var degree in degrees)
            {
                // Average Age
                var EAvgAge = await _context.Database.SqlQueryRaw<decimal?>(
                    "SELECT AVG (1.0 * CASE WHEN Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(Employee_DOB) ELSE 0 END) AS Value FROM dbo.Employee WHERE Degree = @p0",
                    new SqlParameter("@p0", degree ?? (object)DBNull.Value)
                ).FirstOrDefaultAsync() ?? 0; // Use 0 as default if null

                // Minimum Age
                var EMinAge = await _context.Database.SqlQueryRaw<int?>(
                    "SELECT MIN( CASE WHEN Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(Employee_DOB) ELSE 0 END) AS Value FROM dbo.Employee WHERE Degree = @p0",
                    new SqlParameter("@p0", degree ?? (object)DBNull.Value)
                ).FirstOrDefaultAsync() ?? 0;

                // Maximum Age
                var EMaxAge = await _context.Database.SqlQueryRaw<int?>(
                    "SELECT MAX ( CASE WHEN Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(Employee_DOB) ELSE 0 END) AS Value FROM dbo.Employee WHERE Degree = @p0",
                    new SqlParameter("@p0", degree ?? (object)DBNull.Value)
                ).FirstOrDefaultAsync() ?? 0;

                // Count
                var ECount = await _context.Database.SqlQueryRaw<int>(
                    "SELECT COUNT(1) AS Value FROM dbo.Employee WHERE Degree = @p0",
                    new SqlParameter("@p0", degree ?? (object)DBNull.Value)
                ).FirstOrDefaultAsync();

                // Assign results based on the degree
                switch (degree)
                {
                    case "HIGH SCHOOL DIPLOMA":
                        Report.EmployeeAvgAgeHS = EAvgAge;
                        Report.EmployeeMinAgeHS = EMinAge;
                        Report.EmployeeMaxAgeHS = EMaxAge;
                        Report.EmployeeCountHS = ECount;
                        break;
                    case "MASTERS DEGREE":
                        Report.EmployeeAvgAgeMasters = EAvgAge;
                        Report.EmployeeMinAgeMasters = EMinAge;
                        Report.EmployeeMaxAgeMasters = EMaxAge;
                        Report.EmployeeCountMasters = ECount;
                        break;
                    case "BACHELORS DEGREE":
                        Report.EmployeeAvgAgeBachelors = EAvgAge;
                        Report.EmployeeMinAgeBachelors = EMinAge;
                        Report.EmployeeMaxAgeBachelors = EMaxAge;
                        Report.EmployeeCountBachelors = ECount;
                        break;
                    case "DOCTORAL DEGREE":
                        Report.EmployeeAvgAgeDoctoral = EAvgAge;
                        Report.EmployeeMinAgeDoctoral = EMinAge;
                        Report.EmployeeMaxAgeDoctoral = EMaxAge;
                        Report.EmployeeCountDoctoral = ECount;
                        break;
                    case "ASSOCIATE DEGREE":
                        Report.EmployeeAvgAgeAssociates = EAvgAge;
                        Report.EmployeeMinAgeAssociates = EMinAge;
                        Report.EmployeeMaxAgeAssociates = EMaxAge;
                        Report.EmployeeCountAssociates = ECount;
                        break;
                    case null:
                        Report.EmployeeAvgAgeNull = EAvgAge;
                        Report.EmployeeMinAgeNull = EMinAge;
                        Report.EmployeeMaxAgeNull = EMaxAge;
                        Report.EmployeeCountNull = ECount;
                        break;
                }
            }
        }
        public class ReportData
        {
            public decimal EmployeeCountHS { get; set; }
            public decimal EmployeeAvgAgeHS { get; set; }
            public double EmployeeMinAgeHS { get; set; }
            public double EmployeeMaxAgeHS { get; set; }

            public decimal EmployeeCountMasters { get; set; }
            public decimal EmployeeAvgAgeMasters { get; set; }
            public double EmployeeMinAgeMasters { get; set; }
            public double EmployeeMaxAgeMasters { get; set; }

            public decimal EmployeeCountBachelors { get; set; }
            public decimal EmployeeAvgAgeBachelors { get; set; }
            public double EmployeeMinAgeBachelors { get; set; }
            public double EmployeeMaxAgeBachelors { get; set; }

            public decimal EmployeeCountDoctoral { get; set; }
            public decimal EmployeeAvgAgeDoctoral { get; set; }
            public double EmployeeMinAgeDoctoral { get; set; }
            public double EmployeeMaxAgeDoctoral { get; set; }

            public decimal EmployeeCountAssociates { get; set; }
            public decimal EmployeeAvgAgeAssociates { get; set; }
            public double EmployeeMinAgeAssociates { get; set; }
            public double EmployeeMaxAgeAssociates { get; set; }

            public decimal EmployeeCountNull { get; set; }
            public decimal EmployeeAvgAgeNull { get; set; }
            public double EmployeeMinAgeNull { get; set; }
            public double EmployeeMaxAgeNull { get; set; }

            // Other properties...
        }

    }
}
