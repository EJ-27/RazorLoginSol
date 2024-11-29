using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RazorLogin.Models;

public partial class FoodStore
{
    [Key]
    [Display(Name = "FoodStore Id")]
    public int FoodStoreId { get; set; }

    [Display(Name = "Name")]
    public string FoodStoreName { get; set; } = null!;

    [Display(Name = "Address")]
    public string FoodStoreLocation { get; set; } = null!;

    [Display(Name = "Open Time")]
    public TimeOnly? FoodStoreOpenTime { get; set; }

    [Display(Name = "Close Time")]
    public TimeOnly? FoodStoreCloseTime { get; set; }

    [Display(Name = "Capacity")]
    public int? FoodStoreCustomerCapacity { get; set; }

    public string? FoodStoreType { get; set; }

    [Display(Name = "YTD Sales")]
    public int FoodStoreYearToDateSales { get; set; }

    public bool? IsDeleted { get; set; }


    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>(); //change 11/9


    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    //public virtual ICollection<Item> Itemss { get; set; } = new List<Item>(); //change 11/9
    public virtual ICollection<Item> ItemsNavigation { get; set; } = new List<Item>();

}
