using System;
using System.Collections.Generic;

namespace RazorLogin.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string? ItemName { get; set; }

    public int ItemCount { get; set; }

    public DateOnly RestockDate { get; set; }

    public int? ShopId { get; set; }

    public int? FoodStoreId { get; set; }

    public int? ItemPrice { get; set; } //change 11/9

    public bool? IsDeleted { get; set; }


    public virtual FoodStore? FoodStore { get; set; } //change 11/9

    public virtual GiftShop? Shop { get; set; } //change 11/9

    public virtual ICollection<FoodStore> FoodStores { get; set; } = new List<FoodStore>();

    public virtual ICollection<GiftShop> Shops { get; set; } = new List<GiftShop>();
}
