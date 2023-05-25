using Microsoft.AspNetCore.Mvc;
using Sport_Shop.Models;
using Sport_Shop.Areas.Customer.Models;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Sport_Shop.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using System;

namespace Sport_Shop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class Web_StoreController : Controller
    {
        private readonly ShopSportDbConText Database_ShopSport;
        public Web_StoreController(ShopSportDbConText database)
        {
            this.Database_ShopSport = database;
        }
        public async Task<IActionResult> Index([FromQuery] int page)
        {
            int begin = 0;
            if (page == 1 || page == 0 || page == null)
            {
                ViewBag.Page = 1;
                begin = 0;
            }
            else
            {
                ViewBag.Page=page;
                begin = (page * 15) - 15;
            }    
            var listProduct=await Database_ShopSport.Products.Where(x=>x.HoatDong==1 && x.GiamGia!=0).ToListAsync();
            int dem = listProduct.Count();
            int count = Convert.ToInt32((Convert.ToSingle(dem) / 15) + 1.0);
            ViewBag.Count = count;

            var Showproduct= await Database_ShopSport.Products.Where(x => x.HoatDong == 1 && x.GiamGia != 0).Skip(begin).Take(15).ToListAsync();
            return View(Showproduct);
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
            await Database_ShopSport.Customers.AddAsync(new Models.Customer()
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
                HttpContext.Session.SetString("Username", username);
                if(find.Anh==null)
                    HttpContext.Session.SetString("imageUrl","");
                else
                    HttpContext.Session.SetString("imageUrl", find.Anh);
                return new JsonResult(new
                {
                    success = true,
                    redirectToUrl = Url.Action("Index", "Web_Store")
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            HttpContext.Session.SetString("Username","");
            return RedirectToAction("Index");
        }    
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            /*if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Index","Web_Store");*/
            var findcustomer = await Database_ShopSport.Customers.FirstOrDefaultAsync(x => x.TaiKhoan == HttpContext.Session.GetString("Username"));
            return View(findcustomer);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfileCustomer(IFormFile file)
        {
            var form= Request.Form;
            var findcustomer = await Database_ShopSport.Customers.FirstOrDefaultAsync(x => x.TaiKhoan == HttpContext.Session.GetString("Username"));
            
            if(file==null)
            {
                findcustomer.HoTen = form["HoTen"].ToString();
                findcustomer.Phone = form["Phone"].ToString();
                findcustomer.GioiTinh = form["GioiTinh"].ToString();
                findcustomer.NgaySinh = DateTime.Parse(form["NgaySinh"].ToString());
                findcustomer.DiaChi = form["DiaChi"].ToString();
            }    
            else
            {
                string imageUrl = null;
                if (findcustomer.Anh==null || findcustomer.Anh=="")
                {
                    var cloudinary = new Cloudinary(new Account("dbnr304ms", "337989117255642", "YPMNJzY1UjhFz20QC2_kr8mfqr0"));
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream()),
                        PublicId = findcustomer.TaiKhoan,
                    };
                    var uploadResult = cloudinary.Upload(uploadParams);
                    //Transformation
                    cloudinary.Api.UrlImgUp.Transform(new Transformation().Width(100).Height(150).Crop("fill")).BuildUrl("olympic_flag");
                    imageUrl = uploadResult.SecureUrl.ToString();
                }
                else
                {
                    var cloudinary = new Cloudinary(new Account("dbnr304ms", "337989117255642", "YPMNJzY1UjhFz20QC2_kr8mfqr0"));
                    var deletedinary = new DeletionParams(findcustomer.TaiKhoan);
                    var result = await cloudinary.DestroyAsync(deletedinary);
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream()),
                        PublicId = findcustomer.TaiKhoan,
                    };
                    var uploadResult = cloudinary.Upload(uploadParams);
                    //Transformation
                    cloudinary.Api.UrlImgUp.Transform(new Transformation().Width(100).Height(150).Crop("fill")).BuildUrl("olympic_flag");
                    imageUrl = uploadResult.SecureUrl.ToString();
                }    
                findcustomer.Anh = imageUrl;

            }
            if (findcustomer.Anh == null)
                HttpContext.Session.SetString("imageUrl", "");
            else
                HttpContext.Session.SetString("imageUrl", findcustomer.Anh);
            Database_ShopSport.Update(findcustomer);
            await Database_ShopSport.SaveChangesAsync();
            return Json(findcustomer);
        }

        [HttpGet]
        public async Task<IActionResult> EditPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EditPasswordUser()
        {
            var form = Request.Form;
            var matkhaucu= form["MatKhauHienTai"].ToString();
            var matkhaucu2 = form["MatKhauMoi"].ToString();
            var matkhaucu1= form["ConfirmMatKhau"].ToString();

            var findcustomer = await Database_ShopSport.Customers.FirstOrDefaultAsync(x => x.TaiKhoan == HttpContext.Session.GetString("Username"));
            if (findcustomer.MatKhau != Encrypt.ConvertToEncrypt(form["MatKhauHienTai"].ToString()))
                return new JsonResult(new
                {
                    success = false,
                    message = "Mật khẩu cũ không đúng"
                });
            findcustomer.MatKhau = Encrypt.ConvertToEncrypt(form["MatKhauMoi"].ToString());
            Database_ShopSport.Customers.Update(findcustomer);
            await Database_ShopSport.SaveChangesAsync();
            return new JsonResult(new
            {
                success = true,
                message = "Đổi mật khẩu thành công"
            });
        }
        [HttpGet]
        public async Task<IActionResult> SearchProduct(string searchproduct)
        {
            ViewBag.Category=await Database_ShopSport.Category.ToListAsync();
            ViewBag.ListProductSearch=await Database_ShopSport.Products.Where(x=>x.TenSanPham.Contains(searchproduct) && x.HoatDong==1).ToListAsync();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> SearchProduct_Category(int id)
        {
            var listcategory = await Database_ShopSport.Category.Where(c=>c.CategoryId==id)
                .Join(Database_ShopSport.Products,
                    category => category.CategoryId,
                    product => product.CategoryId,
                    (category, product) => new {
                        MaSP=product.ProductId,
                        TenSanPham = product.TenSanPham,
                        Anh = product.Anh,
                        Gia =product.Gia,
                        GiamGia = product.GiamGia,
                    })
                .ToListAsync();
            return Json(listcategory);
        }
        [HttpGet]
        public async Task<IActionResult> ProductDetail([FromRoute] string id)
        {
            ViewBag.Product=await Database_ShopSport.Products.FirstOrDefaultAsync(x=>x.ProductId==int.Parse(id));
            ViewBag.ListSize = await Database_ShopSport.productSizes.Where(c => c.ProductId == int.Parse(id))
                .Join(Database_ShopSport.Sizes,
                    productsize => productsize.SizeId,
                    size => size.SizeId,
                    (productsize, size) => new {
                        SizeId=size.SizeId,
                        TenSize=size.TenSize
                    })
                .ToListAsync();
            return View();
        }






        //giỏ hàng
        public List<Cart> Carts
        {
            get
            {
                var data = HttpContext.Session.Get<List<Cart>>("GioHang");
                if(data==null)
                {
                    data=new List<Cart>();
                }
                return data;
            }
        }
        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            return View(Carts);
        }
        public async Task<IActionResult> addCart(int id,string size,int soluong)
        {
            if (HttpContext.Session.GetString("Username") == null || HttpContext.Session.GetString("Username") == "")
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Vui lòng đăng nhập"
                });
            }



            var product = await Database_ShopSport.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            var myCart = Carts;
            var item = myCart.FirstOrDefault(x => x.ProductId == id && x.TenSize==size);
            if(item==null)
            {
                item = new Cart()
                {
                    
                    ProductId = id,
                    Gia = product.Gia - (product.Gia * product.GiamGia / 100),
                    Image = product.Anh,
                    SoLuong = soluong,
                    TenSanPham = product.TenSanPham,
                    TenSize = size
                };
                if(myCart.Count==0)
                {
                    item.CartID = 1;
                }    
                else
                {
                    item.CartID = myCart.Max(x => x.CartID) + 1;
                }    
                myCart.Add(item);
            }    
            else
            {
                item.SoLuong+=soluong;
            }
            HttpContext.Session.Set("GioHang",myCart);
            return new JsonResult(new
            {
                success = true,
                message = "Thêm vào giỏ hàng thành công"
            });
        }
        [HttpGet]
        public async Task<IActionResult> GiamSoLuong([FromRoute]int id)
        {
            var mycart = Carts;
            int vitri = mycart.FindIndex(x => x.CartID == id);
            if(mycart[vitri].SoLuong>1)
                mycart[vitri].SoLuong--;
            HttpContext.Session.Set("GioHang", mycart);
            return RedirectToAction("ViewCart");
        }
        [HttpGet]
        public async Task<IActionResult> TangSoLuong([FromRoute] int id)
        {
            var mycart = Carts;
            int vitri = mycart.FindIndex(x => x.CartID == id);
            mycart[vitri].SoLuong++;
            HttpContext.Session.Set("GioHang", mycart);
            return RedirectToAction("ViewCart");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteCartById([FromRoute] int id)
        {
            var mycart = Carts;
            int vitri = mycart.FindIndex(x => x.CartID == id);
            mycart.Remove(mycart[vitri]);
            HttpContext.Session.Set("GioHang", mycart);
            return RedirectToAction("ViewCart");
        }
        [HttpGet]
        public async Task<IActionResult> OrderByNow(string TenSanPham,int GiaBan,int SoLuong,string TenSize,string DiaChi,string NguoiNhan, string Phone)
        {
            if (HttpContext.Session.GetString("Username") == null || HttpContext.Session.GetString("Username") == "")
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Vui lòng đăng nhập",
                    redirectToUrl = Url.Action("Index", "Web_Store")
                });
            }
            if(DiaChi==null || NguoiNhan==null || Phone==null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Vui lòng điền đủ thông tin nhận hàng",
                });
            }

            var findcustomer = await Database_ShopSport.Customers.FirstOrDefaultAsync(x => x.TaiKhoan == HttpContext.Session.GetString("Username"));
            var order = new Order()
            {
                CustomerId = findcustomer.CustomerId,
                DiaChi = DiaChi,
                NgayDat = DateTime.Now,
                NguoiNhan = NguoiNhan,
                Phone = Phone,
                TotalPrice = GiaBan*SoLuong,
            };
            await Database_ShopSport.orders.AddAsync(order);
            await Database_ShopSport.SaveChangesAsync();



            int lastIdOrder = await Database_ShopSport.orders.MaxAsync(x => x.OrderId);
            var findsize = await Database_ShopSport.Sizes.FirstOrDefaultAsync(x => x.TenSize == TenSize);
            var findproduct = await Database_ShopSport.Products.FirstOrDefaultAsync(x => x.TenSanPham == TenSanPham);
            var orderdetai = new OrderDetail()
            {
                OrderId = lastIdOrder,
                Gia = GiaBan,
                ProductId = findproduct.ProductId,
                SoLuong =SoLuong,
                SizeId = findsize.SizeId,
            };
            await Database_ShopSport.orderDetails.AddAsync(orderdetai);
            await Database_ShopSport.SaveChangesAsync();



            return new JsonResult(new
            {
                success = true,
                message = "Đặt Hàng thành công",
                redirectToUrl = Url.Action("ViewOrder", "Web_Store")
            });
        }
        [HttpPost]
        public async Task<IActionResult> Order()
        {
            

            var form = Request.Form;
            

            var mycart = Carts;
            int totalPrice = 0;
            foreach(var item in mycart)
            {
                totalPrice += item.Gia * item.SoLuong;
            }    
            var findcustomer = await Database_ShopSport.Customers.FirstOrDefaultAsync(x => x.TaiKhoan == HttpContext.Session.GetString("Username"));
            var order = new Order()
            {
                CustomerId = findcustomer.CustomerId,
                DiaChi = form["DiaChi"].ToString(),
                NgayDat = DateTime.Now,
                NguoiNhan = form["NguoiNhan"].ToString(),
                Phone = form["Phone"].ToString(),
                TotalPrice = totalPrice,
            };
            await Database_ShopSport.orders.AddAsync(order);
            await Database_ShopSport.SaveChangesAsync();

            int lastIdOrder = await Database_ShopSport.orders.MaxAsync(x => x.OrderId);
            foreach (var item in mycart)
            {
                var findsize = await Database_ShopSport.Sizes.FirstOrDefaultAsync(x => x.TenSize == item.TenSize);
                var orderdetai = new OrderDetail()
                {
                    OrderId= lastIdOrder,
                    Gia=item.Gia,
                    ProductId=item.ProductId,
                    SoLuong=item.SoLuong,
                    SizeId=findsize.SizeId,
                };
                await Database_ShopSport.orderDetails.AddAsync(orderdetai);
                await Database_ShopSport.SaveChangesAsync();
            }
            mycart=null;
            HttpContext.Session.Set("GioHang", mycart);
            return new JsonResult(new
            {
                success = true,
                message = "Đặt Hàng thành công",
                redirectToUrl = Url.Action("ViewOrder", "Web_Store")
            });
        }
        [HttpGet]    
        public async Task<IActionResult> ViewOrder()
        {
            var fincustoemr = await Database_ShopSport.Customers.FirstOrDefaultAsync(x=>x.TaiKhoan==HttpContext.Session.GetString("Username"));
            var list = await Database_ShopSport.orders.Where(x => x.CustomerId ==fincustoemr.CustomerId).OrderByDescending(data => data.OrderId).ToListAsync();
            return View(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetOrderDetailByOrderId(int madh)
        {
            var List = from e in Database_ShopSport.orderDetails
                       join d in Database_ShopSport.Products on e.ProductId equals d.ProductId
                       join f in Database_ShopSport.Sizes on e.SizeId equals f.SizeId
                       where e.OrderId == madh
                       select new
                       {
                           TenSanPham = d.TenSanPham,
                           TenSize = f.TenSize,
                           Anh = d.Anh,
                           SoLuong = e.SoLuong,
                           Gia = e.Gia,
                       };
            return Json(List);
        }
        [HttpGet]
        public async Task<IActionResult> DaNhanHang(int madh)
        {
            var findorder = await Database_ShopSport.orders.FirstOrDefaultAsync(x => x.OrderId == madh);
            findorder.TrangThai = 4;
            Database_ShopSport.Update(findorder);
            await Database_ShopSport.SaveChangesAsync();
            return new JsonResult(new
            {
                redirectToUrl = Url.Action("ViewOrder", "Web_Store")
            });
        }
        [HttpGet]
        public async Task<IActionResult> HuyDonHang(int madh)
        {
            var findorder = await Database_ShopSport.orders.FirstOrDefaultAsync(x => x.OrderId == madh);
            findorder.TrangThai = -1;
            Database_ShopSport.Update(findorder);
            await Database_ShopSport.SaveChangesAsync();
            return new JsonResult(new
            {
                redirectToUrl = Url.Action("ViewOrder", "Web_Store")
            });
        }


    }
}
