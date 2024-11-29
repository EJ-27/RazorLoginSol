/*using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace RazorLogin.Pages.Admin.Reports3
{
    public class ManIndexModel : PageModel
    {

        private readonly RazorLogin.Models.ZooDbContext _context;

        public ManIndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

        public ReportData Report { get; set; }

        public async Task OnGetAsync()
        Report = new ReportData();

        var degrees = new[]
        {
                "HIGH SCHOOL DIPLOMA",
                "MASTERS DEGREE",
                "BACHELORS DEGREE",
                "DOCTORAL DEGREE",
                "ASSOCIATE DEGREE",
                null
        };

            foreach (var degree in degrees)
            {
                // Join with Employee to get degree information
                var EAvgAge = await _context.Database.SqlQueryRaw<decimal?>(
                    "SELECT AVG(YEAR(GETDATE()) - YEAR(E.Employee_DOB)) FROM dbo.Manager AS M JOIN dbo.Employee AS E ON M.EmployeeId = E.EmployeeId WHERE E.Degree = @p0",
                    new SqlParameter("@p0", degree ?? (object)DBNull.Value)
                ).FirstOrDefaultAsync() ?? 0;

        var ECount = await _context.Database.SqlQueryRaw<int>(
            "SELECT COUNT(*) FROM dbo.Manager AS M JOIN dbo.Employee AS E ON M.EmployeeId = E.EmployeeId WHERE E.Degree = @p0",
            new SqlParameter("@p0", degree ?? (object)DBNull.Value)
        ).FirstOrDefaultAsync();

                // Assign results based on the degree
                switch (degree)
                {
                    case "HIGH SCHOOL DIPLOMA":
                        Report.ManagerCountHS = ECount;
                        Report.ManagerAvgAgeHS = EAvgAge;
                        break;
                    // Add cases for other degrees...
                }

        
    }

            public class ReportData
        {
            public decimal ManagerCountHS { get; set; }
            public decimal ManagerAvgAgeHS { get; set; }
            // Add other properties as needed...
        }
    
}*/