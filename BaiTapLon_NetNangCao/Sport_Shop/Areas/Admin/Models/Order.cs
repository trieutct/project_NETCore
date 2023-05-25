using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Sport_Shop.Areas.Customer.Models;

namespace Sport_Shop.Areas.Admin.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int TotalPrice { get; set; }
        public string NguoiNhan { get; set; }
        public string Phone { get; set; }
        public string DiaChi { get; set; }
        public DateTime NgayDat { get; set; }
        public int TrangThai { get; set; } //0 là chờ duyệt     1 là hoàn thành
        public Sport_Shop.Areas.Customer.Models.Customer customer { get; set; }
        public List<OrderDetail>? orderDetails { get; set; }
    }
}
