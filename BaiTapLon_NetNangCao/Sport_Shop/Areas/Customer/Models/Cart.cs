namespace Sport_Shop.Areas.Customer.Models
{
    public class Cart
    {
        public int CartID { get; set; }
        public int ProductId { get; set; }
        public string TenSanPham { get; set; }
        public string Image { get; set; }
        public int Gia { get; set; }
        public int SoLuong { get; set; }
        public string TenSize { get; set; }
    }
}
