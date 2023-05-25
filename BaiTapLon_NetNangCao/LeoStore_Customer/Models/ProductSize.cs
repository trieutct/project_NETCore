using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sport_Shop.Areas.Admin.Models
{
    public class ProductSize
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductSizeId { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int SoLuongTon { get; set; }
        public Size? size { get; set; }
        public Product? product { get; set; }

    }
}
