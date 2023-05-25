using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Sport_Shop.Areas.Customer.Models;

namespace Sport_Shop.Areas.Admin.Models
{
    [Table("Product")]
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm!")]
        public string TenSanPham { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả sản phẩm!")]
        public string MoTa { get; set; }
        public string? Anh { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá bán sản phẩm!")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public int Gia { get; set; }
        [Range(0,1, ErrorMessage = "Chỉ còn có và không!!!")]
        public int HoatDong { get; set; } //1 là còn, 0 là không
        [Range(0,100,ErrorMessage ="Giảm giá chỉ từ 0% đến 100%")]
        public int GiamGia { get; set; }
        public string TrangThai { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
        public List<ProductSize>? productSizes { get; set; }
        public List<OrderDetail>? orderDetails { get; set; }
    }
}
