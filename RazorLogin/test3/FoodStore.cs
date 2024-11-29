using System;
using System.Collections.Generic;

namespace RazorLogin.test3;

public partial class FoodStore
{
    public int FoodStoreId { get; set; }

    public string FoodStoreName { get; set; } = null!;

    public string FoodStoreLocation { get; set; } = null!;

    public TimeOnly? FoodStoreOpenTime { get; set; }

    public TimeOnly? FoodStoreCloseTime { get; set; }

    public int? FoodStoreCustomerCapacity { get; set; }

    public string? FoodStoreType { get; set; }

    public int FoodStoreYearToDateSales { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Item> ItemsNavigation { get; set; } = new List<Item>();
}
