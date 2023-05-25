using Sport_Shop.Areas.Customer.Models;

namespace Sport_Shop.Areas.Admin.Models
{
    public class Size
    {
        public int SizeId { get; set; }
        public string TenSize { get; set; }
        public List<ProductSize>? productSizes { get; set; }
        public List<OrderDetail>? orderDetails { get; set; }
    }
}
