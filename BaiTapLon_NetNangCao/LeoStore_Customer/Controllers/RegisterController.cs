using LeoStore_Customer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sport_Shop.Models;
using System.Security.Cryptography.Xml;

namespace LeoStore_Customer.Controllers
{
    public class RegisterController : Controller
    {
        private readonly DbConTextDatabase Database_ShopSport;
        public RegisterController(DbConTextDatabase database)
        {
            this.Database_ShopSport = database;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterCustomerModel userModel)
        {
            if(!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    TempData["Message"] = "<script>window.onload = function () {alert('"+error.ErrorMessage+"');}</script>";
                    break;
                }
                return View();
            }
            else
            {
                if(await CheckUserCustomer(userModel.TaiKhoan))
                {
                    return View();
                }
                else
                {
                    var customer = new Customer()
                    {
                        TaiKhoan = userModel.TaiKhoan,
                        MatKhau = Encrypt.ConvertToEncrypt(userModel.MatKhau),
                        NgayTao = DateTime.Now,
                    };
                    await Database_ShopSport.Customers.AddAsync(customer);
                    await Database_ShopSport.SaveChangesAsync();
                    return Ok("Thành công: tài khoản "+customer.TaiKhoan+"  Mật khẩu: "+customer.MatKhau);
                }
            }    
            
        }
        [NonAction]
        public async Task<bool> CheckUserCustomer(string TaiKhoan)
        {
            var check = await Database_ShopSport.Customers.FirstOrDefaultAsync(x => x.TaiKhoan == TaiKhoan);
            if (check!=null)
            {
                TempData["Message"] = "<script>window.onload = function () {alert('Tài khoản đã tồn tại');}</script>";
                return true;
            }    
            return false;
        }
    }
}
