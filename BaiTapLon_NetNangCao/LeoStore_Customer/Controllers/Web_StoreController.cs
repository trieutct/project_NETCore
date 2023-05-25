using LeoStore_Customer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sport_Shop.Models;
using Microsoft.AspNetCore.Http;

namespace LeoStore_Customer.Controllers
{
    public class Web_StoreController : Controller
    {
        private readonly DbConTextDatabase Database_ShopSport;
        public Web_StoreController(DbConTextDatabase database)
        {
            this.Database_ShopSport = database;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CheckRegister(string username, string password, string cfpassword)
        {
            if (username == null || password == null || cfpassword == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Không được để trống"
                });
            }
            else if (password.Length < 5 || cfpassword.Length < 5)
            {
                return Json(new
                {
                    success = false,
                    message = "Mật khẩu phải có trên 5 kí tự"
                });
            }

            else if (password != cfpassword)
            {
                return Json(new
                {
                    success = false,
                    message = "2 mật khẩu không giống nhau"
                });
            }
            else if (await CheckUsernameCustomerInDatabase(username))
            {
                return Json(new
                {
                    success = false,
                    message = "Tài khoản đã tồn tại"
                });
            }
            await Database_ShopSport.Customers.AddAsync(new Customer()
            {
                TaiKhoan = username,
                MatKhau = Encrypt.ConvertToEncrypt(password),
                NgayTao = DateTime.Now
            });
            await Database_ShopSport.SaveChangesAsync();
            return Json(new
            {
                success = true,
                message = "Đăng ký tài khoản thành công"
            });
        }
        [NonAction]
        public async Task<bool> CheckUsernameCustomerInDatabase(string taikhoan)
        {
            var check = await Database_ShopSport.Customers.FirstOrDefaultAsync(x => x.TaiKhoan == taikhoan);
            if (check != null)
                return true;
            return false;
        }
        [HttpGet]
        public async Task<IActionResult> CheckLogin(string username, string password)
        {
            if (username == null || password == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng nhập tài khoản mật khẩu để đăng nhập"
                });
            }
            var find = await Database_ShopSport.Customers.FirstOrDefaultAsync(x => x.TaiKhoan == username && x.MatKhau == Encrypt.ConvertToEncrypt(password));
            if (find == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Tài khoản mật khẩu sai"
                });
            }
            else
            {
                HttpContext.Session.SetString("Username",username);
                return new JsonResult(new
                {
                    success = true,
                    redirectToUrl = Url.Action("Index", "Web_Store")
                }) ;
            }
        }

    }
}
