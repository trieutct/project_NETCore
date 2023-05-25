using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Sport_Shop.Areas.Admin.Models;

namespace Sport_Shop.Areas.Customer.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }
        [MaxLength(100)]
        public string? HoTen { get; set; }
        [MaxLength(10)]
        public string? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        [MaxLength(15)]
        public string? Phone { get; set; }
        [Column(TypeName = "Varchar(300)")]
        public string? Anh { get; set; }
        [MaxLength(200)]
        public string? DiaChi { get; set; }
        public DateTime NgayTao { get; set; }
        [MinLength(5)]
        public string TaiKhoan { get; set; }
        [MinLength(4)]
        public string MatKhau { get; set; }
        public List<Order>? orders { get; set; }
    }
}
