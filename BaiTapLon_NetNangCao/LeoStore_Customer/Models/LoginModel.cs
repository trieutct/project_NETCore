using System.ComponentModel.DataAnnotations;

namespace LeoStore_Customer.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tài khoản !")]
        [MinLength(5, ErrorMessage = "Tài khoản quá ngắn!")]
        public string TaiKhoan { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu !")]
        [MinLength(5, ErrorMessage = "Mật khẩu phải trên 5 kí tự")]
        public string MatKhau { get; set; }
    }
}
