using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopApi.Models;

[Table("ShoppingCart")]
public partial class ShoppingCart
{
    [Key]
    public int Id { get; set; }

    [Column("User_id")]
    public int UserId { get; set; }

    [Column("Product_id")]
    public int ProductId { get; set; }

    public int Quantity { get; set; }

}
