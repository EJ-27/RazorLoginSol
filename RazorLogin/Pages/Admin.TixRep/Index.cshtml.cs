using RazorLogin.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RazorLogin.Data;
using System.Net.Sockets;
using Humanizer;
using RazorLogin.Pages.Admin.Reports;

namespace RazorLogin.Pages.Admin.TixRep
{
    public class IndexModel : PageModel
    {
        private readonly ZooDbContext _context;

        public IndexModel(ZooDbContext context)
        {
            _context = context;
        }

        // Properties for the input date range
        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; }

        // Property to hold the report result
        //public List<TicketReport> Report { get; set; }
        public TicketReport Report { get; set; }


        // OnGet method to process the report generation
        public async Task OnGetAsync()
        {
            // Ensure the report is initialized if it's null
            if (Report == null)
            {
                Report = new TicketReport();
            }

            // Initialize the membership counts lists to avoid null reference issues
            Report.ChildMembershipCounts = new List<TicketMembershipCount>();
            Report.AdultMembershipCounts = new List<TicketMembershipCount>();
            Report.SeniorMembershipCounts = new List<TicketMembershipCount>();
            Report.VeteranMembershipCounts = new List<TicketMembershipCount>();

            Report.TickCostsByTick = new List<TicketCost>();
            
            if (StartDate != DateTime.MinValue && EndDate != DateTime.MinValue && StartDate <= EndDate)
            {
                // Convert DateTime to DateOnly for SQL
                DateOnly startDateOnly = DateOnly.FromDateTime(StartDate);
                DateOnly endDateOnly = DateOnly.FromDateTime(EndDate);


                // 1. Get Ticket Count for each Ticket Type (Child, Adult, Senior, etc.)
                var ChildCT = await _context.Database.SqlQueryRaw<int>($"SELECT COUNT(*) AS Value FROM dbo.Ticket t JOIN Purchase p ON t.Purchase_ID = p.Purchase_ID WHERE t.Ticket_Type = 'Child' AND t.Ticket_Purchase_date BETWEEN {{0}} AND {{1}}",  startDateOnly.ToString(), endDateOnly.ToString()).FirstOrDefaultAsync();
                Report.ChildCTR = ChildCT;

                var AdultCT = _context.Database.SqlQueryRaw<int>($"SELECT COUNT(*) AS Value FROM dbo.Ticket t JOIN Purchase p ON t.Purchase_ID = p.Purchase_ID WHERE t.Ticket_Type = 'Adult' AND t.Ticket_Purchase_date BETWEEN {{0}} AND {{1}}",  startDateOnly.ToString(), endDateOnly.ToString()).FirstOrDefault();
                Report.AdultCTR = AdultCT;

                var SeniorCT = _context.Database.SqlQueryRaw<int>($"SELECT COUNT(*) AS Value FROM dbo.Ticket t JOIN Purchase p ON t.Purchase_ID = p.Purchase_ID WHERE t.Ticket_Type = 'Senior' AND t.Ticket_Purchase_date BETWEEN {{0}} AND {{1}}",  startDateOnly.ToString(), endDateOnly.ToString()).FirstOrDefault();
                Report.SeniorCTR = SeniorCT;

                var VeteranCT = _context.Database.SqlQueryRaw<int>($"SELECT COUNT(*) AS Value FROM dbo.Ticket t JOIN Purchase p ON t.Purchase_ID = p.Purchase_ID WHERE t.Ticket_Type = 'Veteran' AND t.Ticket_Purchase_date BETWEEN {{0}} AND {{1}}",  startDateOnly.ToString(), endDateOnly.ToString()).FirstOrDefault();
                Report.VeteranCTR = VeteranCT;



                // 2. Get Average Customer Age
                var ChildTixAvgAge = await _context.Database.SqlQueryRaw<decimal?>(@"SELECT 1.0 * AVG(DATEDIFF(YEAR, c.Customer_DOB, GETDATE())) AS Value   FROM dbo.Ticket t  JOIN Purchase p ON t.Purchase_ID = p.Purchase_ID  JOIN Customer c ON p.Customer_ID = c.Customer_ID  WHERE t.Ticket_Type = 'Child'  AND t.Ticket_Purchase_date BETWEEN {0} AND {1}", startDateOnly.ToString(), endDateOnly.ToString()).FirstOrDefaultAsync();
                Report.ChildAvgAge = ChildTixAvgAge;

                var AdultTixAvgAge = await _context.Database.SqlQueryRaw<decimal?>(@"SELECT  1.0 * AVG(DATEDIFF(YEAR, c.Customer_DOB, GETDATE())) AS Value   FROM dbo.Ticket t  JOIN Purchase p ON t.Purchase_ID = p.Purchase_ID  JOIN Customer c ON p.Customer_ID = c.Customer_ID  WHERE t.Ticket_Type = 'Adult'  AND t.Ticket_Purchase_date BETWEEN {0} AND {1}", startDateOnly.ToString(), endDateOnly.ToString()).FirstOrDefaultAsync();
                Report.AdultAvgAge = AdultTixAvgAge;

                var SeniorTixAvgAge = await _context.Database.SqlQueryRaw<decimal?>(@"SELECT  1.0 * AVG(DATEDIFF(YEAR, c.Customer_DOB, GETDATE())) AS Value   FROM dbo.Ticket t  JOIN Purchase p ON t.Purchase_ID = p.Purchase_ID  JOIN Customer c ON p.Customer_ID = c.Customer_ID  WHERE t.Ticket_Type = 'Senior'  AND t.Ticket_Purchase_date BETWEEN {0} AND {1}", startDateOnly.ToString(), endDateOnly.ToString()).FirstOrDefaultAsync();
                Report.SeniorAvgAge = SeniorTixAvgAge;

                var VeteranTixAvgAge = await _context.Database.SqlQueryRaw<decimal?>(@"SELECT  1.0 * AVG(DATEDIFF(YEAR, c.Customer_DOB, GETDATE())) AS Value   FROM dbo.Ticket t  JOIN Purchase p ON t.Purchase_ID = p.Purchase_ID  JOIN Customer c ON p.Customer_ID = c.Customer_ID  WHERE t.Ticket_Type = 'Veteran'  AND t.Ticket_Purchase_date BETWEEN {0} AND {1}", startDateOnly.ToString(), endDateOnly.ToString()).FirstOrDefaultAsync();
                Report.VeteranAvgAge = VeteranTixAvgAge;


                // Query to get membership count for each ticket type
                var ChildmembershipCounts = await _context.Database.SqlQueryRaw<TicketMembershipCount>(@"
                    SELECT 
                        'Child' AS Ticket_Type, 
                        m.Membership_type,
                        COUNT(t.Ticket_ID) AS TicketCount
                    FROM 
                        (SELECT 'FREE TIER' AS Membership_type
                         UNION ALL 
                         SELECT 'FAMILY TIER' 
                         UNION ALL 
                         SELECT 'VIP TIER') AS m 
                    LEFT JOIN 
                        dbo.Customer c ON 1 = 1  
                    LEFT JOIN 
                        dbo.Purchase p ON p.Customer_ID = c.Customer_ID  
                    LEFT JOIN 
                        dbo.Ticket t ON t.Purchase_ID = p.Purchase_ID AND t.Ticket_Type = 'Child' 
                    WHERE
                        c.Membership_type = m.Membership_type 
                        AND t.Ticket_Purchase_date BETWEEN {0} AND {1}
                    GROUP BY 
                        m.Membership_type
                    ORDER BY 
                        m.Membership_type;
                ", startDateOnly.ToString("yyyy-MM-dd"), endDateOnly.ToString("yyyy-MM-dd")).ToListAsync();

                // Store result in the Report object
                Report.ChildMembershipCounts = ChildmembershipCounts;

                //////////////////////////
                

                // Query to get membership count for each ticket type
                var AdultmembershipCounts = await _context.Database.SqlQueryRaw<TicketMembershipCount>(@"
                    SELECT 
                        'Adult' AS Ticket_Type, 
                        m.Membership_type,
                        COUNT(t.Ticket_ID) AS TicketCount
                    FROM 
                        (SELECT 'FREE TIER' AS Membership_type
                         UNION ALL 
                         SELECT 'FAMILY TIER' 
                         UNION ALL 
                         SELECT 'VIP TIER') AS m 
                    LEFT JOIN 
                        dbo.Customer c ON 1 = 1  
                    LEFT JOIN 
                        dbo.Purchase p ON p.Customer_ID = c.Customer_ID  
                    LEFT JOIN 
                        dbo.Ticket t ON t.Purchase_ID = p.Purchase_ID AND t.Ticket_Type = 'Adult' 
                    WHERE
                        c.Membership_type = m.Membership_type 
                        AND t.Ticket_Purchase_date BETWEEN {0} AND {1}
                    GROUP BY 
                        m.Membership_type
                    ORDER BY 
                        m.Membership_type;
                ", startDateOnly.ToString("yyyy-MM-dd"), endDateOnly.ToString("yyyy-MM-dd")).ToListAsync();

                // Store result in the Report object
                Report.AdultMembershipCounts = AdultmembershipCounts;

                //////////////////////////
                

                // Query to get membership count for each ticket type
                var SeniormembershipCounts = await _context.Database.SqlQueryRaw<TicketMembershipCount>(@"
                    SELECT 
                        'Senior' AS Ticket_Type, 
                        m.Membership_type,
                        COUNT(t.Ticket_ID) AS TicketCount
                    FROM 
                        (SELECT 'FREE TIER' AS Membership_type
                         UNION ALL 
                         SELECT 'FAMILY TIER' 
                         UNION ALL 
                         SELECT 'VIP TIER') AS m 
                    LEFT JOIN 
                        dbo.Customer c ON 1 = 1  
                    LEFT JOIN 
                        dbo.Purchase p ON p.Customer_ID = c.Customer_ID  
                    LEFT JOIN 
                        dbo.Ticket t ON t.Purchase_ID = p.Purchase_ID AND t.Ticket_Type = 'Senior' 
                    WHERE
                        c.Membership_type = m.Membership_type 
                        AND t.Ticket_Purchase_date BETWEEN {0} AND {1}
                    GROUP BY 
                        m.Membership_type
                    ORDER BY 
                        m.Membership_type;
                ", startDateOnly.ToString("yyyy-MM-dd"), endDateOnly.ToString("yyyy-MM-dd")).ToListAsync();

                // Store result in the Report object
                Report.SeniorMembershipCounts = SeniormembershipCounts;

                //////////////////////////////
                

                // Query to get membership count for each ticket type
                var VeteranmembershipCounts = await _context.Database.SqlQueryRaw<TicketMembershipCount>(@"
                    SELECT 
                        'Veteran' AS Ticket_Type, 
                        m.Membership_type,
                        COUNT(t.Ticket_ID) AS TicketCount
                    FROM 
                        (SELECT 'FREE TIER' AS Membership_type
                         UNION ALL 
                         SELECT 'FAMILY TIER' 
                         UNION ALL 
                         SELECT 'VIP TIER') AS m 
                    LEFT JOIN 
                        dbo.Customer c ON 1 = 1  
                    LEFT JOIN 
                        dbo.Purchase p ON p.Customer_ID = c.Customer_ID  
                    LEFT JOIN 
                        dbo.Ticket t ON t.Purchase_ID = p.Purchase_ID AND t.Ticket_Type = 'Veteran' 
                    WHERE
                        c.Membership_type = m.Membership_type 
                        AND t.Ticket_Purchase_date BETWEEN {0} AND {1}
                    GROUP BY 
                        m.Membership_type
                    ORDER BY 
                        m.Membership_type;
                ", startDateOnly.ToString("yyyy-MM-dd"), endDateOnly.ToString("yyyy-MM-dd")).ToListAsync();

                // Store result in the Report object
                Report.VeteranMembershipCounts = VeteranmembershipCounts;


                // Ticket cost by ticket type

                var TickCostType = await _context.Database.SqlQueryRaw<TicketCost>(@"
                    SELECT 
                        SUM(CASE WHEN t.Ticket_Type = 'Child' THEN t.Ticket_Price ELSE 0 END) AS ChildTotal,
                        SUM(CASE WHEN t.Ticket_Type = 'Adult' THEN t.Ticket_Price ELSE 0 END) AS AdultTotal,
                        SUM(CASE WHEN t.Ticket_Type = 'Senior' THEN t.Ticket_Price ELSE 0 END) AS SeniorTotal,
                        SUM(CASE WHEN t.Ticket_Type = 'Veteran' THEN t.Ticket_Price ELSE 0 END) AS VeteranTotal
                    FROM dbo.Ticket t
                    JOIN dbo.Purchase p ON t.Purchase_ID = p.Purchase_ID
                    WHERE t.Ticket_Purchase_Date BETWEEN {0} AND {1}
                ", startDateOnly.ToString("yyyy-MM-dd"), endDateOnly.ToString("yyyy-MM-dd")).ToListAsync();
                                
                Report.TickCostsByTick = TickCostType;

            }

        }

        public class TicketCost
        { 
            public int? ChildTotal { get; set; }
            public int? AdultTotal { get; set; }
            public int? SeniorTotal { get; set; }
            public int? VeteranTotal { get; set; }
        }

        public class TicketMembershipCount
        {
            // Update to match SQL column names
            public string Ticket_Type { get; set; } 


            public string Membership_type { get; set; } 


            public int TicketCount { get; set; } 
        }

        public class TicketReport
        {
            public int ChildCTR { get; set; }
            public int AdultCTR {  get; set; }
            public int SeniorCTR {  get; set; }
            public int VeteranCTR {  get; set; }

            public decimal? ChildAvgAge { get; set; }
            public decimal? AdultAvgAge { get; set; }
            public decimal? VeteranAvgAge { get; set; }
            public decimal? SeniorAvgAge { get; set; }

            public List<TicketMembershipCount> ChildMembershipCounts { get; set; }
            public List<TicketMembershipCount> AdultMembershipCounts { get; set; }
            public List<TicketMembershipCount> SeniorMembershipCounts { get; set; }
            public List<TicketMembershipCount> VeteranMembershipCounts { get; set; }

            public List<TicketCost> TickCostsByTick { get; set; }
        }
    }
}
