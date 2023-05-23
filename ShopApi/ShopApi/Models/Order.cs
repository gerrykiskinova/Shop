using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopApi.Models;

public partial class Order
{
    [Key]
    public int Id { get; set; }

    [Column("Order_name")]
    public string OrderName{ get; set; }

    [Column("User_id")]
    public int UserId { get; set; }

    [Column("Order_date", TypeName = "date")]
    public DateTime OrderDate { get; set; }

    [Column("Total_amount", TypeName = "decimal(10, 2)")]
    public decimal TotalAmount { get; set; }

}
