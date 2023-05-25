using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sport_Shop.Areas.Admin.Models;
using Sport_Shop.Models;
using Microsoft.AspNetCore.Http;

namespace Sport_Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly ShopSportDbConText  Database_ShopSport;
        public LoginController(ShopSportDbConText database)
        {
            this.Database_ShopSport = database;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("EmployyeeAccountId") != null)
            {
                HttpContext.Session.Clear();
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(EmployeeLogin ee)
        {

            if(!CheckEmployeeAccount(ee.UserName,ee.Password))
            {
                TempData["Message"] = "<script>window.onload = function () {alert('Vui lòng nhập thông tin cần đăng nhập');}</script>";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                if(!await CheckEmployeeDatabase(ee.UserName,ee.Password))
                {
                    TempData["Message"] = "<script>window.onload = function () {alert('Sai tài khoản mật khẩu');}</script>";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    var search = from e in Database_ShopSport.EmployeeAccount
                                 join d in Database_ShopSport.Employee on e.EmployeeAccountId equals d.EmployeeAccountId
                                 where e.TaiKhoan == ee.UserName
                                 select new
                                 {
                                     tenanh = d.Anh,
                                     chucvu=d.ChucVu,
                                     accountid=e.EmployeeAccountId
                                 };
                    foreach (var item in search)
                    {
                        HttpContext.Session.SetString("UrlIamge", item.tenanh);
                        HttpContext.Session.SetInt32("EmployyeeAccountId", item.accountid);
                        HttpContext.Session.SetInt32("ChucVu", item.chucvu);
                    }
                    //return Ok(Encrypt.ConvertToEncrypt(ee.Password));
                    return RedirectToAction("Index", "Admin", new {page=1});
                }
            }
        }
        [NonAction]
        public bool CheckEmployeeAccount(string UserName,string Password)
        {
            if(string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                return false;
            }    
            return true;
        }
        [NonAction]
        public async Task<bool> CheckEmployeeDatabase(string UserName, string Password)
        {
            var employee =await Database_ShopSport.EmployeeAccount.SingleOrDefaultAsync(x => x.TaiKhoan == UserName && x.MatKhau == Encrypt.ConvertToEncrypt(Password));
            if(employee!=null)
            {
                return true;
            }    
            return false;
        }
    }
}
