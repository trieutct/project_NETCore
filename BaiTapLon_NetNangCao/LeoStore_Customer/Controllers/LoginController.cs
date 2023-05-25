using LeoStore_Customer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sport_Shop.Models;

namespace LeoStore_Customer.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbConTextDatabase Database_ShopSport;
        public LoginController(DbConTextDatabase database)
        {
            this.Database_ShopSport = database;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel account)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    TempData["Message"] = "<script>window.onload = function () {alert('" + error.ErrorMessage + "');}</script>";
                    break;
                }
                return RedirectToAction("Index", "Login");
            }
            if (await CheckTaiKhoanMatKhau(account.TaiKhoan,account.MatKhau))
            {
                return Ok("Đăng nhập thành công");
            }
            TempData["Message"] = "<script>window.onload = function () {alert('Sai tài khoản mật khẩu');}</script>";
            return RedirectToAction("Index", "Login");
        }
        [NonAction]
        public async Task<bool> CheckTaiKhoanMatKhau(string taikhona,string matkhau)
        {
            var account = await Database_ShopSport.Customers.FirstOrDefaultAsync(x => x.TaiKhoan == taikhona && x.MatKhau == Encrypt.ConvertToEncrypt(matkhau));
            if (account != null)
                return true;
            return false;
        }
    }
}
