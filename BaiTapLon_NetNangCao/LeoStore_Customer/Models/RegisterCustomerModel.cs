using System.ComponentModel.DataAnnotations;
namespace LeoStore_Customer.Models
{
    public class RegisterCustomerModel
    {
        [Required(ErrorMessage ="Không được để trống!")]
        [MinLength(5,ErrorMessage ="Tài khoản quá ngắn!")]
        public string TaiKhoan { get; set; }
        [Required(ErrorMessage = "Không được để trống!")]
        [MinLength(5,ErrorMessage ="Mật khẩu phải trên 5 kí tự")]
        public string MatKhau { get; set; }
        [Required(ErrorMessage ="Không được để trống!")]
        [Compare("MatKhau",ErrorMessage ="Hai mật khẩu không giống nhau")]
        public string CfMatKhau { get; set; }

    }
}
