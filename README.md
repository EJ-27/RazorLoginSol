# RazorLoginSol

ASP.NET EF CORE USING C# and RAZOR

- the customer can register and log into their accounnt. An employee's account will be made by admin. 

LOG INS... <br />
ADMIN LOG IN: Admin@Official.com  password = Test1234! <br />
MANAGER LOG IN: onlinemanager@zoo.com  password = Test1234! <br />
ZOOKEEPER LOG IN: Zookeeper@Zookeeper.com  password = Test1234! <br />
CUSTOMER LOG IN:  mina@customer.com  password = Test1234! <br />
SHOP LOG IN: Employee2@Employee2.com  password = Test1234!


Steps to run the website:<br />

    1.Clone the repository using Visual Studio 2022 
    2.In Visual Studio 2022, click "File -> Open -> Project Solution" 
    3.Navigate to the cloned project folder and open file "RazorLoginSol.sln" 
    4.Start the project 

All c# and accompannying html razor files can be found within the pages folder.<br />
All models of sql tables can be found in the Models folder, including the dbcontext, ZooDbContext.<br />
dbo.Trigger.sql is defunct and reamins for documentation.<br />

<br /><br /><br /> ALL CODE FILES CAN BE FOUND IN RazorLogin FOLDER <br /><br />

TRIGGERS<br />
1. Can be found in 'closing' table of SSMS backpak. Triggered in zookeeper view when a 'closing; is created for the furture. Sets the associated enclosure to 'CLOSED' <br />
2. Can be found in 'Event' table of SSMS backpak. Triggered in Employee and Zookeeper view when an employee/zookeeper adds more than 2 events to the DB, rejects.
<br /><br />

QUERIES <br />
1. Can be found in Employee view in Events tab. Allows user to filter events by date and view the associated employees. Pulls from Events table and Employees table. <br />
2. Can be found in manager view in Finance Reports tab --> Ticket Report tab. Allows user to filter by date for ticket sales and view customer information in relation to the sales. pulls from customer table and ticket table. <br />
3. Can be found in manager view in Finance Reports tab --> Purchase Sales Report tab. Allows user to filter by date for Purchase sales and view customer information in relation to the sales. pulls from customer table and Purchases table.
    
