using System;
using System.Collections.Generic;

namespace RazorLogin.test3;

public partial class GiftShop
{
    public int ShopId { get; set; }

    public string? GiftShopName { get; set; }

    public string? GiftShopLocation { get; set; }

    public int GiftShopYearToDateSales { get; set; }

    public TimeOnly GiftShopOpenTime { get; set; }

    public TimeOnly GiftShopCloseTime { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Item> ItemsNavigation { get; set; } = new List<Item>();
}
