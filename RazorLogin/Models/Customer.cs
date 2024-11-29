using System;
using System.Collections.Generic;

namespace RazorLogin.Models;

public partial class Customer
{

    public int CustomerId { get; set; }

    public string? CustomerFirstName { get; set; }

    public string? CustomerLastName { get; set; }

    public DateOnly? CustomerDob { get; set; }

    public string? CustomerAddress { get; set; }

    public DateOnly MembershipStartDate { get; set; }

    public DateOnly MembershipEndDate { get; set; }

    public string? MembershipType { get; set; }

    public string CustomerEmail { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public virtual Purchase? Purchase { get; set; }

    public virtual ICollection<FoodStore> FoodStores { get; set; } = new List<FoodStore>();

    public virtual ICollection<GiftShop> Shops { get; set; } = new List<GiftShop>();
}
