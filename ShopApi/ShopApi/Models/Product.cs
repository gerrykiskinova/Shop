using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ShopApi.Models;

public partial class Product
{
    [Key]
    public int Id { get; set; }

    [Column("Creator_id")]
    public int CreatorId { get; set; }

    [Column("Product_name")]
    [StringLength(100)]
    [Unicode(false)]
    public string ProductName { get; set; } = null!;

    [Column("Product_pic")]
    [StringLength(50)]
    public string ProductPic { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [NotMapped]
    public IFormFile? Image { get; set; }
}
