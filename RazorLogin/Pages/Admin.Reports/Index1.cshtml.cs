
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;
using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace RazorLogin.Pages.Admin.Reports
{
    public class Index1Model : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public Index1Model(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }


        public ReportData Report { get; set; }


        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }


        // Calculated properties
        public int TotalDays { get; set; }
        public double TotalHealthcareIncome { get; set; }
        public double Tier1Income { get; set; }
        public double Tier2Income { get; set; }
        public double Tier3Income { get; set; }
        public int totalEmployeeSalary { get; set; }

        // Tier counts (example values, replace with actual logic)
        public int Tier1CT { get; set; }
        public int Tier2CT { get; set; }
        public int Tier3CT { get; set; }

        //decimal totalEmployeeSalary = 0;

        public int TimeSpanInDays { get; set; } // user-defined days


        public async Task OnGetAsync()
        {

            Report = new ReportData();

            if (StartDate.HasValue && EndDate.HasValue)
            {
                // Calculate the difference in days
                TimeSpanInDays = (EndDate.Value - StartDate.Value).Days - 1;
            }
            else
            {
                // Handle the case where one or both dates are missing
                Console.WriteLine("Start Date or End Date is missing");
                TimeSpanInDays = 0; // You can default to 0 or any other value
            }

            // Employee Stats

            var EAvgAge = _context.Database.SqlQuery<decimal>($"SELECT AVG(1.0 * CASE WHEN Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(Employee_DOB) ELSE 0 END) AS Value FROM dbo.Employee").FirstOrDefault();
            Report.EmployeeAvgAge = (decimal)EAvgAge;
            var EMinAge = _context.Database.SqlQuery<int>($"SELECT MIN(CASE WHEN Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(Employee_DOB) ELSE 0 END) AS Value FROM dbo.Employee").FirstOrDefault();
            Report.EmployeeMinAge = (double)EMinAge;
            var EMaxAge = _context.Database.SqlQuery<int>($"SELECT MAX(CASE WHEN Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(Employee_DOB) ELSE 0 END) AS Value FROM dbo.Employee").FirstOrDefault();
            Report.EmployeeMaxAge = (double)EMaxAge;

            var EAvgSal = _context.Database.SqlQuery<decimal>($"SELECT AVG(1.0 * CASE WHEN Employee_Salary IS NOT NULL THEN Employee_Salary ELSE 0 END) AS Value FROM dbo.Employee").FirstOrDefault();
            Report.EmployeeAvgSalary = (decimal)EAvgSal;
            var EMinSal = _context.Database.SqlQuery<int>($"SELECT MIN(CASE WHEN Employee_Salary IS NOT NULL THEN Employee_Salary ELSE 0 END) AS Value FROM dbo.Employee").FirstOrDefault();
            Report.EmployeeMinSalary = (double)EMinSal;
            var EMaxSal = _context.Database.SqlQuery<int>($"SELECT MAX(CASE WHEN Employee_Salary IS NOT NULL THEN Employee_Salary ELSE 0 END) AS Value FROM dbo.Employee").FirstOrDefault();
            Report.EmployeeMaxSalary = (double)EMaxSal;

            var ETotSal = _context.Database.SqlQuery<int>($"SELECT SUM(CASE WHEN Employee_Salary IS NOT NULL THEN Employee_Salary ELSE 0 END) AS Value FROM dbo.Employee").FirstOrDefault();
            Report.totalEmployeeSalary = (int)(ETotSal * (TimeSpanInDays / 365.0)); 



            //Dependent

            var Tier1 = _context.Database.SqlQuery<double?>($"SELECT 1.0 * CAST(SUM(70) AS FLOAT) AS Value FROM dbo.Dependant WHERE Healthcare_tier = 'Tier 1'").FirstOrDefault();
            var Tier1CT = _context.Database.SqlQuery<int?>($"SELECT COUNT(*) AS Value FROM dbo.Dependant WHERE Healthcare_tier = 'Tier 1'").FirstOrDefault();
            Report.Tier1CT = (double?)Tier1CT ?? 0;
            Report.Tier1Income = (Tier1 ?? 0) * (TimeSpanInDays / 30.0);

            var Tier2 = _context.Database.SqlQuery<double?>($"SELECT 1.0 * CAST(SUM(140) AS FLOAT) AS Value FROM dbo.Dependant WHERE Healthcare_tier = 'Tier 2'").FirstOrDefault();
            var Tier2CT = _context.Database.SqlQuery<int?>($"SELECT COUNT(*) AS Value FROM dbo.Dependant WHERE Healthcare_tier = 'Tier 2'").FirstOrDefault();
            Report.Tier2CT = (double?)Tier2CT ?? 0;
            Report.Tier2Income = (Tier2 ?? 0) * (TimeSpanInDays / 30.0); 

            var Tier3 = _context.Database.SqlQuery<double?>($"SELECT 1.0 * CAST(SUM(200) AS FLOAT) AS Value FROM dbo.Dependant WHERE Healthcare_tier = 'Tier 3'").FirstOrDefault();
            var Tier3CT = _context.Database.SqlQuery<int?>($"SELECT COUNT(*) AS Value FROM dbo.Dependant WHERE Healthcare_tier = 'Tier 3'").FirstOrDefault();
            Report.Tier3CT = (double?)Tier3CT ?? 0;
            Report.Tier3Income = (Tier3 ?? 0) * (TimeSpanInDays / 30.0); 


            // Calculate total healthcare income
            double totalHealthcareIncome = Report.Tier1Income + Report.Tier2Income + Report.Tier3Income;

            // Subtract total healthcare income from total employee salaries
            double difference = Report.totalEmployeeSalary - totalHealthcareIncome;

            // Store this difference in the Report
            Report.TotalDifference = difference;



            // Manager stats

            var MAvgAge = _context.Database.SqlQuery<decimal>($"SELECT AVG(1.0 * CASE WHEN e.Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(e.Employee_DOB) ELSE NULL END) AS Value  FROM dbo.Manager m JOIN dbo.Employee e ON m.Employee_ID = e.Employee_ID").FirstOrDefault();
            Report.ManagerAvgAge = (decimal)MAvgAge;
            var MMinAge = _context.Database.SqlQuery<int>($"SELECT MIN( CASE WHEN e.Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(e.Employee_DOB) ELSE NULL END) AS Value  FROM dbo.Manager m JOIN dbo.Employee e ON m.Employee_ID = e.Employee_ID").FirstOrDefault();
            Report.ManagerMinAge = (double)MMinAge;
            var MMaxAge = _context.Database.SqlQuery<int>($"SELECT MAX(CASE WHEN e.Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(e.Employee_DOB) ELSE NULL END) AS Value  FROM dbo.Manager m JOIN dbo.Employee e ON m.Employee_ID = e.Employee_ID").FirstOrDefault();
            Report.ManagerMaxAge = (double)MMaxAge;

            var MAvgSal = _context.Database.SqlQuery<decimal>($"SELECT AVG(1.0 * CASE WHEN Employee_Salary IS NOT NULL THEN Employee_Salary ELSE 0 END) AS Value FROM dbo.Manager m JOIN dbo.Employee e ON m.Employee_ID = e.Employee_ID").FirstOrDefault();
            Report.ManagerAvgSalary = (decimal)MAvgSal;
            var MMinSal = _context.Database.SqlQuery<int>($"SELECT MIN(CASE WHEN Employee_Salary IS NOT NULL THEN Employee_Salary ELSE 0 END) AS Value FROM dbo.Manager m JOIN dbo.Employee e ON m.Employee_ID = e.Employee_ID").FirstOrDefault();
            Report.ManagerMinSalary = (double)MMinSal;
            var NMaxSal = _context.Database.SqlQuery<int>($"SELECT MAX(CASE WHEN Employee_Salary IS NOT NULL THEN Employee_Salary ELSE 0 END) AS Value FROM dbo.Manager m JOIN dbo.Employee e ON m.Employee_ID = e.Employee_ID").FirstOrDefault();
            Report.ManagerMaxSalary = (double)NMaxSal;




            // Zookeeper stats

            var ZAvgAge = _context.Database.SqlQuery<decimal>($"SELECT AVG(1.0 * CASE WHEN e.Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(e.Employee_DOB) ELSE NULL END) AS Value  FROM dbo.Zookeeper m JOIN dbo.Employee e ON m.Employee_ID = e.Employee_ID").FirstOrDefault();
            Report.ZookeeperAvgAge = (decimal)ZAvgAge;
            var ZMinAge = _context.Database.SqlQuery<int>($"SELECT MIN( CASE WHEN e.Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(e.Employee_DOB) ELSE NULL END) AS Value  FROM dbo.Zookeeper m JOIN dbo.Employee e ON m.Employee_ID = e.Employee_ID").FirstOrDefault();
            Report.ZookeeperMinAge = (double)ZMinAge;
            var ZMaxAge = _context.Database.SqlQuery<int>($"SELECT MAX(CASE WHEN e.Employee_DOB IS NOT NULL THEN YEAR(GETDATE()) - YEAR(e.Employee_DOB) ELSE NULL END) AS Value  FROM dbo.Zookeeper m JOIN dbo.Employee e ON m.Employee_ID = e.Employee_ID").FirstOrDefault();
            Report.ZookeeperMaxAge = (double)ZMaxAge;

            var ZAvgSal = _context.Database.SqlQuery<decimal>($"SELECT AVG(1.0 * CASE WHEN Employee_Salary IS NOT NULL THEN Employee_Salary ELSE 0 END) AS Value FROM dbo.Zookeeper m JOIN dbo.Employee e ON m.Employee_ID = e.Employee_ID").FirstOrDefault();
            Report.ZookeeperAvgSalary = (decimal)ZAvgSal;
            var ZMinSal = _context.Database.SqlQuery<int>($"SELECT MIN(CASE WHEN Employee_Salary IS NOT NULL THEN Employee_Salary ELSE 0 END) AS Value FROM dbo.Zookeeper m JOIN dbo.Employee e ON m.Employee_ID = e.Employee_ID").FirstOrDefault();
            Report.ZookeeperMinSalary = (double)ZMinSal;
            var ZMaxSal = _context.Database.SqlQuery<int>($"SELECT MAX(CASE WHEN Employee_Salary IS NOT NULL THEN Employee_Salary ELSE 0 END) AS Value FROM dbo.Zookeeper m JOIN dbo.Employee e ON m.Employee_ID = e.Employee_ID").FirstOrDefault();
            Report.ZookeeperMaxSalary = (double)ZMaxSal;

        }
    }
    public class ReportData
    {
        public double TotalDifference { get; set; }

        public int totalEmployeeSalary { get; set; }
        public decimal EmployeeAvgAge { get; set; }
        public double EmployeeMinAge { get; set; }
        public double EmployeeMaxAge { get; set; }
        public decimal EmployeeAvgSalary { get; set; }
        public double EmployeeMinSalary { get; set; }
        public double EmployeeMaxSalary { get; set; }
        public double Tier1Income { get; set; }
        public double Tier2Income { get; set; }
        public double Tier3Income { get; set; }
        public double Tier1CT { get; set; }
        public double Tier2CT { get; set; }
        public double Tier3CT { get; set; }

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
        public int TimeSpanInDays { get; set; }
    }
}


