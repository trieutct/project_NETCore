using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Sport_Shop.Areas.Admin.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập tên danh mục!")]
        [MinLength(3,ErrorMessage ="Vui lòng nhập đúng!")]
        public string TenDanhMuc { get; set; }
        public virtual IList<Product>? Products{ get; set;}
    }
}
