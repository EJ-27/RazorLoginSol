using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RazorLogin.Models;

namespace RazorLogin.Pages.Admin.Emp
{
    public class CreateModel : PageModel
    {
        private readonly RazorLogin.Models.ZooDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(RazorLogin.Models.ZooDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }



        public IActionResult OnGet()
        {

            // Get a list of Gift Shops that do NOT have managers assigned to them
            var GiftShopsWithoutManagers = _context.GiftShops
                .Where(shop => !_context.Managers
                    .Any(mgr => mgr.EmployeeId != null &&
                                _context.Employees
                                    .Where(emp => emp.EmployeeId == mgr.EmployeeId)
                                    .Any(emp => emp.ShopId == shop.ShopId))) // No manager assigned to this shop
                .Select(shop => new SelectListItem
                {
                    Value = shop.ShopId.ToString(),
                    Text = shop.GiftShopName // Display the shop name (or any other relevant field)
                })
                .ToList();


            // Get a list of Food Stores that do NOT have managers assigned to them
            var FoodStoresWithoutManagers = _context.FoodStores
                .Where(store => !_context.Managers
                    .Any(mgr => mgr.EmployeeId != null &&
                                _context.Employees
                                    .Where(emp => emp.EmployeeId == mgr.EmployeeId)
                                    .Any(emp => emp.FoodStoreId == store.FoodStoreId))) // No manager assigned to this food store
                .Select(store => new SelectListItem
                {
                    Value = store.FoodStoreId.ToString(),
                    Text = store.FoodStoreName // Display the store name (or any other relevant field)
                })
                .ToList();


            ///////////////////////////////////////////////////////////////////////////////////////

            // Get a list of Gift Shops that have managers assigned to them
            var giftShopsWithManagers = _context.Managers
                .Where(mgr => mgr.EmployeeId != null)  // Make sure the manager has an employee linked
                .Join(_context.Employees, // Join the Manager table with the Employee table
                    mgr => mgr.EmployeeId, // Using EmployeeId as the key
                    emp => emp.EmployeeId,
                    (mgr, emp) => new { emp.ShopId }) // Project the ShopId from Employee table
                .Where(x => x.ShopId.HasValue) // Only include records where ShopId is not null
                .Select(x => x.ShopId.Value)  // Select the ShopId to filter the stores
                .Distinct() // Get distinct ShopIds (to avoid duplicate stores)
                .ToList();

            // Get a list of Food Stores that have managers assigned to them
            var foodStoresWithManagers = _context.Managers
                .Where(mgr => mgr.EmployeeId != null)  // Make sure the manager has an employee linked
                .Join(_context.Employees, // Join the Manager table with the Employee table
                    mgr => mgr.EmployeeId, // Using EmployeeId as the key
                    emp => emp.EmployeeId,
                    (mgr, emp) => new { emp.FoodStoreId }) // Project the FoodStoreId from Employee table
                .Where(x => x.FoodStoreId.HasValue) // Only include records where FoodStoreId is not null
                .Select(x => x.FoodStoreId.Value)  // Select the FoodStoreId to filter the stores
                .Distinct() // Get distinct FoodStoreIds (to avoid duplicate stores)
                .ToList();

            // Now, fetch the Gift Shops that have managers assigned
            var GiftShopsWithManagers = _context.GiftShops
                .Where(shop => giftShopsWithManagers.Contains(shop.ShopId)) // Only those that have managers
                .Select(shop => new SelectListItem
                {
                    Value = shop.ShopId.ToString(),
                    Text = shop.GiftShopName // You can use other fields for display as well
                })
                .ToList();

            // Now, fetch the Food Stores that have managers assigned
            var FoodStoresWithManagers = _context.FoodStores
                .Where(store => foodStoresWithManagers.Contains(store.FoodStoreId)) // Only those that have managers
                .Select(store => new SelectListItem
                {
                    Value = store.FoodStoreId.ToString(),
                    Text = store.FoodStoreName // You can use other fields for display as well
                })
                .ToList();


            // Add default options for both Gift Shops and Food Stores
            GiftShopsWithManagers.Insert(0, new SelectListItem { Value = "", Text = "--- FOR EMPLOYEES ---" });
            FoodStoresWithManagers.Insert(0, new SelectListItem { Value = "", Text = "--- FOR EMPLOYEES ---" });

            GiftShopsWithoutManagers.Insert(0, new SelectListItem { Value = "", Text = "--- FOR MANAGERS ---" });
            FoodStoresWithoutManagers.Insert(0, new SelectListItem { Value = "", Text = "--- FOR MANAGERS ---" });

            // Pass the filtered stores to the view
            ViewData["GiftShopsWithManagers"] = GiftShopsWithManagers;
            ViewData["FoodStoresWithManagers"] = FoodStoresWithManagers;


            ViewData["GiftShopsWithoutManagers"] = GiftShopsWithoutManagers;
            ViewData["FoodStoresWithoutManagers"] = FoodStoresWithoutManagers;



            // Populate supervisor selection (you may already have this set correctly)
            List<SelectListItem> selectList = _context.FoodStores
                .AsNoTracking()
                .Select(x => new SelectListItem()
                {
                    Value = x.FoodStoreId.ToString(),
                })
                .ToList();

            selectList.Insert(0, new SelectListItem()
            {
                Value = "",
                Text = "--- Select Related Entity ---"
            });

            ViewData["RelatedEntity_Id"] = selectList;

            // You can populate ViewData for SupervisorId (Manager)
            ViewData["SupervisorId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");

            return Page();


            /* ViewData["FoodStoreId"] = new SelectList(_context.FoodStores, "FoodStoreId", "FoodStoreId");

                List<SelectListItem> selectList = _context.FoodStores
                    .AsNoTracking()
                    .Select(x => new SelectListItem()
                    {
                        Value = x.FoodStoreId.ToString(),

                    })
                    .ToList();

                selectList.Insert(0, new SelectListItem()
                {
                    Value = "",
                    Text = "--- Select Related Entity ---"
                });

                ViewData["RelatedEntity_Id"] = selectList;

                ViewData["ShopId"] = new SelectList(_context.GiftShops, "ShopId", "ShopId");
                ViewData["SupervisorId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
                return Page(); */

        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Role { get; set; }

        public string ErrorMessage { get; set; } // Store the error message for display

        // Password validation method
        private bool ValidatePassword(string password)
        {
            if (password.Length < 6)
            {
                return false; // Password is too short
            }

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasNonAlphanumeric = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpperCase && hasDigit && hasNonAlphanumeric;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Step 1: Manually validate the password
            if (!ValidatePassword(Password))
            {
                ModelState.AddModelError(string.Empty, "Password must contain at least one uppercase letter, one digit, one non-alphanumeric character, and be at least 6 characters long.");
                return Page();
            }

            // Step 2: Check if both a FoodStore and a GiftShop are selected
            if ((Employee.FoodStoreId.HasValue && Employee.FoodStoreId.Value != 0) &&
                (Employee.ShopId.HasValue && Employee.ShopId.Value != 0))
            {
                ModelState.AddModelError(string.Empty, "An employee cannot be assigned to both a Food Store and a Gift Shop. Please select only one.");
                return Page();
            }

            try
            {
                // Set department based on role
                if (Role == "ZOOKEEPER")
                {
                    Employee.Department = "ZOO";
                }
                else if (Role == "ADMIN")
                {
                    Employee.Department = "GENERAL";
                }
                else if (Role == "SHOP")
                {
                    if (!string.IsNullOrEmpty(Employee.ShopId.ToString()) && Employee.ShopId != 0)
                    {
                        Employee.Department = "GIFT";

                        // Set the supervisor for Gift Shop
                        var managerForGiftShop = _context.Managers
                            .FirstOrDefault(mgr => mgr.EmployeeId != null && mgr.Employee.ShopId == Employee.ShopId);
                        if (managerForGiftShop != null)
                        {
                            Employee.SupervisorId = managerForGiftShop.ManagerId;
                        }
                    }
                    else if (!string.IsNullOrEmpty(Employee.FoodStoreId.ToString()) && Employee.FoodStoreId != 0)
                    {
                        Employee.Department = "FOOD";

                        // Set the supervisor for Food Store
                        var managerForFoodStore = _context.Managers
                            .FirstOrDefault(mgr => mgr.EmployeeId != null && mgr.Employee.FoodStoreId == Employee.FoodStoreId);
                        if (managerForFoodStore != null)
                        {
                            Employee.SupervisorId = managerForFoodStore.ManagerId;
                        }
                    }
                }
                else if (Role == "MANAGER")
                {
                    if (!string.IsNullOrEmpty(Employee.ShopId.ToString()) && Employee.ShopId != 0)
                    {
                        Employee.Department = "GIFT";
                    }
                    else if (!string.IsNullOrEmpty(Employee.FoodStoreId.ToString()) && Employee.FoodStoreId != 0)
                    {
                        Employee.Department = "FOOD";
                    }
                }
                else
                {
                    Employee.Department = "GENERAL";
                }


                // Add employee to context
                _context.Employees.Add(Employee);
                await _context.SaveChangesAsync(); // This might trigger your SQL trigger

                // Create IdentityUser for authentication
                var user = new IdentityUser
                {
                    UserName = Employee.EmployeeEmail,
                    Email = Employee.EmployeeEmail
                };


                var result = await _userManager.CreateAsync(user, Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

              


                if (!string.IsNullOrEmpty(Role))
                {
                    await _userManager.AddToRoleAsync(user, Role);

                    var randomSuffix = new Random().Next(10000, 99999);

                    if (Role == "MANAGER")
                    {
                        var manager = new Manager
                        {
                            EmployeeId = Employee.EmployeeId,
                            ManagerId = int.Parse($"{Employee.EmployeeId}{randomSuffix}"),
                            Department = Employee.Department,
                            ManagerEmploymentDate = DateOnly.FromDateTime(DateTime.Now)
                        };
                        _context.Managers.Add(manager);
                    }
                    else if (Role == "ZOOKEEPER")
                    {
                        var zookeeper = new Zookeeper
                        {
                            EmployeeId = Employee.EmployeeId,
                            ZookeeperId = int.Parse($"{Employee.EmployeeId}{randomSuffix}"),
                            TrainingRenewalDate = DateOnly.FromDateTime(DateTime.Now.AddYears(1))
                        };
                        _context.Zookeepers.Add(zookeeper);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                // Catch errors related to database updates (including trigger errors)
                var sqlException = ex.InnerException as SqlException;
                if (sqlException != null)
                {
                    // Handle SQL error (from trigger)
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the employee: " + sqlException.Message);
                }
                else
                {
                    // General error if not SQL-related
                    ModelState.AddModelError(string.Empty, "An unknown error occurred while saving the employee.");
                }



                // Return to the current page with the error message
                return Page();
            }
        }

    }
}