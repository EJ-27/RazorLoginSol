using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Data;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.PurchRep
{
    public class IndexModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;

        public IndexModel(RazorLogin.Models.ZooDbContext context)
        {
            _context = context;
        }

            [BindProperty(SupportsGet = true)]
            public DateOnly StartDate { get; set; }

            [BindProperty(SupportsGet = true)]
            public DateOnly EndDate { get; set; }

            public int? TotalRevenue { get; set; }
            public int? HighestPurchase { get; set; }
            public int? LowestPurchase { get; set; }
            public List<CustomerReport> CustomerData { get; set; }
            public List<PurchasePerDayReport> PurchasesPerDay { get; set; }

            public async Task OnGetAsync()
            {
                // Default to the past 30 days if no dates are provided
                if (StartDate == null || EndDate == null)
                {
                    StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
                    EndDate = DateOnly.FromDateTime(DateTime.Now);
                }

                    TotalRevenue = await _context.Database.SqlQueryRaw<int?>(@"
                SELECT SUM(p.Total_purchases_price)  AS Value
                FROM dbo.Purchase p
                LEFT JOIN dbo.Ticket t ON t.Purchase_ID = p.Purchase_ID
                WHERE p.Purchase_date BETWEEN {0} AND {1} AND t.Purchase_ID IS NULL
            ", StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")).FirstOrDefaultAsync();

                    HighestPurchase = await _context.Database.SqlQueryRaw<int?>(@"
                SELECT MAX(p.Total_purchases_price) AS Value
                FROM dbo.Purchase p
                LEFT JOIN dbo.Ticket t ON t.Purchase_ID = p.Purchase_ID
                WHERE p.Purchase_date BETWEEN {0} AND {1} AND t.Purchase_ID IS NULL
            ", StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")).FirstOrDefaultAsync();

                    LowestPurchase = await _context.Database.SqlQueryRaw<int?>(@"
                SELECT MIN(p.Total_purchases_price) AS Value
                FROM dbo.Purchase p
                LEFT JOIN dbo.Ticket t ON t.Purchase_ID = p.Purchase_ID
                WHERE p.Purchase_date BETWEEN {0} AND {1} AND t.Purchase_ID IS NULL
            ", StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")).FirstOrDefaultAsync();

            CustomerData = await _context.Database.SqlQueryRaw<CustomerReport>(@"
    SELECT c.Customer_ID AS Customer_ID, 
           DATEDIFF(YEAR, c.Customer_DOB, 
               ISNULL(MAX(p.Purchase_date), '1900-01-01')) - 
               CASE 
                   WHEN MONTH(ISNULL(MAX(p.Purchase_date), '1900-01-01')) < MONTH(c.Customer_DOB) OR 
                        (MONTH(ISNULL(MAX(p.Purchase_date), '1900-01-01')) = MONTH(c.Customer_DOB) AND DAY(ISNULL(MAX(p.Purchase_date), '1900-01-01')) < DAY(c.Customer_DOB)) 
                   THEN 1 ELSE 0 
               END AS CustomerAge,
           c.Membership_type AS Membership_type, 
           ISNULL(SUM(p.Total_purchases_price), 0) AS TotalPurchasesOverall
    FROM dbo.Purchase p
    INNER JOIN dbo.Customer c ON p.Customer_ID = c.Customer_ID
    LEFT JOIN dbo.Ticket t ON t.Purchase_ID = p.Purchase_ID
    WHERE p.Purchase_date BETWEEN {0} AND {1}
    AND t.Purchase_ID IS NULL
    GROUP BY c.Customer_ID, c.Customer_DOB, c.Membership_type
    ORDER BY c.Customer_ID
", StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")).ToListAsync();


            // Fetch Purchases per Day Data
            PurchasesPerDay = await _context.Database.SqlQueryRaw<PurchasePerDayReport>(@"
        SELECT 
            p.Purchase_date AS Purchase_date, 
            COALESCE(COUNT(p.Purchase_ID), 0) AS TotalPurchases
        FROM dbo.Purchase p
        LEFT JOIN dbo.Ticket t ON t.Purchase_ID = p.Purchase_ID
        WHERE p.Purchase_date BETWEEN {0} AND {1} 
        AND t.Purchase_ID IS NULL
        GROUP BY p.Purchase_date
        ORDER BY p.Purchase_date
    ", StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")).ToListAsync();

        }
    }
    public class CustomerReport
    {
        public int Customer_ID { get; set; }  // Instead of CustomerId
        public int? CustomerAge { get; set; }
        public string Membership_type { get; set; }  // Instead of MembershipType
        public int? TotalPurchasesOverall { get; set; }  // Instead of TotalPurchasesOverall
    }


    public class PurchasePerDayReport
    {
        public DateOnly Purchase_date { get; set; }
        public int TotalPurchases { get; set; }
    }
}





