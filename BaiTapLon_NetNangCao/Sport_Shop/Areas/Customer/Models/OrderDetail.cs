using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Sport_Shop.Areas.Admin.Models;

namespace Sport_Shop.Areas.Customer.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int SoLuong { get; set; }
        public int Gia { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
        public Size? size { get; set; }
    }
}
