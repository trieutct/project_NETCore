using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Sport_Shop.Areas.Admin.Models;
using Sport_Shop.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Sport_Shop.Areas.Customer.Models;
using static NuGet.Packaging.PackagingConstants;

namespace Sport_Shop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly ShopSportDbConText Database_ShopSport;
        public AdminController(ShopSportDbConText database)
        {
            this.Database_ShopSport = database;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.SumNhanVien = await Database_ShopSport.Employee.CountAsync();
            ViewBag.SumCustomer = await Database_ShopSport.Customers.CountAsync();
            ViewBag.SumProduct= await Database_ShopSport.Products.CountAsync();
            ViewBag.SumCart = await Database_ShopSport.orders.Where(x => x.TrangThai == 4).CountAsync();
            ViewBag.DoanhThuHomNay = Database_ShopSport.orders
                                 .Join(Database_ShopSport.orderDetails, o => o.OrderId, od => od.OrderId, (o, od) => new { Order = o, OrderDetail = od })
                                 .Where(o => o.Order.NgayDat.Date == DateTime.Today && o.Order.TrangThai==4)
                                 .Sum(od => od.OrderDetail.SoLuong*od.OrderDetail.Gia);


            DateTime startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            DateTime endDate = startDate.AddDays(7);
            ViewBag.DoanhThuTuan = Database_ShopSport.orders
                                 .Join(Database_ShopSport.orderDetails, o => o.OrderId, od => od.OrderId, (o, od) => new { Order = o, OrderDetail = od })
                                 .Where(o => o.Order.NgayDat>= startDate && o.Order.NgayDat < endDate && o.Order.TrangThai == 4)
                                 .Sum(od => od.OrderDetail.SoLuong * od.OrderDetail.Gia);


            ViewBag.DoanhThuThang = Database_ShopSport.orders
                                 .Join(Database_ShopSport.orderDetails, o => o.OrderId, od => od.OrderId, (o, od) => new { Order = o, OrderDetail = od })
                                 .Where(o => o.Order.NgayDat.Month==DateTime.Today.Month && o.Order.TrangThai == 4)
                                 .Sum(od => od.OrderDetail.SoLuong * od.OrderDetail.Gia);

            ViewBag.DoanhThuNam = Database_ShopSport.orders
                                 .Join(Database_ShopSport.orderDetails, o => o.OrderId, od => od.OrderId, (o, od) => new { Order = o, OrderDetail = od })
                                 .Where(o => o.Order.NgayDat.Year == DateTime.Today.Year && o.Order.TrangThai == 4)
                                 .Sum(od => od.OrderDetail.SoLuong * od.OrderDetail.Gia);
            return View();
        }
        public async Task<IActionResult> EmployeeManager([FromQuery] int page)
        {
            //phân trang
            int begin;
            if (page == 0 || page == 1)
            {
                ViewBag.page = 1;
                begin = 0;
            }
            else
            {
                begin = (page * 5) - 5;
                ViewBag.page = page;
            }
            int dem = Database_ShopSport.Employee.Count();
            int count = Convert.ToInt32((Convert.ToSingle(dem) / 5)+1.0);
            ViewBag.Count = count;
            var employees = await Database_ShopSport.Employee.Where(x => x.EmployeeAccountId != HttpContext.Session.GetInt32("EmployyeeAccountId")).Skip(begin).Take(5).ToListAsync();
            return View(employees);
        }
        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployee employee, IFormFile file)
        {
            if (!CheckFormAddEmployee(employee.HoTen, employee.GioiTinh, employee.NgaySinh.ToString(), employee.DiaChi, employee.Phone, employee.NgayVao.ToString(), employee.ChucVu.ToString(), employee.TaiKhoan, employee.MatKhau))
            {
                TempData["Message"] = "<script>window.onload = function () {alert('Vui lòng điền đầy đủ thông tin');}</script>";
                return RedirectToAction("AddEmployee", "Admin");
            }
            else
            {
                if (!await CheckEmployeeAccount_Database(employee.TaiKhoan))
                {
                    if (file == null)
                    {
                        var ee = new EmployeeAccount()
                        {
                            TaiKhoan = employee.TaiKhoan,
                            MatKhau = Encrypt.ConvertToEncrypt(employee.MatKhau)
                        };
                        await Database_ShopSport.EmployeeAccount.AddAsync(ee);
                        await Database_ShopSport.SaveChangesAsync();

                        var empl = new Emoloyee()
                        {
                            HoTen = employee.HoTen,
                            GioiTinh = employee.GioiTinh,
                            NgaySinh = employee.NgaySinh,
                            DiaChi = employee.DiaChi,
                            Phone = employee.Phone,
                            NgayVao = employee.NgayVao,
                            ChucVu = employee.ChucVu,
                            Anh = "",
                            EmployeeAccount = ee
                        };
                        await Database_ShopSport.Employee.AddAsync(empl);
                        await Database_ShopSport.SaveChangesAsync();
                        TempData["Message"] = "<script>window.onload = function () {alert('Thêm thành công');}</script>";
                        return RedirectToAction("EmployeeManager", "Admin");
                    }
                    else
                    {
                        var ee = new EmployeeAccount()
                        {
                            TaiKhoan = employee.TaiKhoan,
                            MatKhau = Encrypt.ConvertToEncrypt(employee.MatKhau)
                        };
                        await Database_ShopSport.EmployeeAccount.AddAsync(ee);
                        await Database_ShopSport.SaveChangesAsync();

                        int getLastEmployeeAccountId = Database_ShopSport.EmployeeAccount.Max(x=>x.EmployeeAccountId);
                        var cloudinary = new Cloudinary(new Account("dgtjdhrnq", "356379962535312", "1kgyT8lBh3odrCT0YV84tiIhKgE"));
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.FileName, file.OpenReadStream()),
                            PublicId = getLastEmployeeAccountId.ToString(),
                        };

                        var uploadResult = cloudinary.Upload(uploadParams);
                        //Transformation
                        cloudinary.Api.UrlImgUp.Transform(new Transformation().Width(100).Height(150).Crop("fill")).BuildUrl("olympic_flag");
                        var imageUrl = uploadResult.SecureUrl.ToString();
                        var empl = new Emoloyee()
                        {
                            HoTen = employee.HoTen,
                            GioiTinh = employee.GioiTinh,
                            NgaySinh = employee.NgaySinh,
                            DiaChi = employee.DiaChi,
                            Phone = employee.Phone,
                            NgayVao = employee.NgayVao,
                            ChucVu = employee.ChucVu,
                            Anh = imageUrl,
                            EmployeeAccount = ee
                        };
                        await Database_ShopSport.Employee.AddAsync(empl);
                        await Database_ShopSport.SaveChangesAsync();
                        TempData["Message"] = "<script>window.onload = function () {alert('Thêm thành công');}</script>";
                        return RedirectToAction("EmployeeManager", "Admin");
                    }
                }
                else
                {
                    TempData["Message"] = "<script>window.onload = function () {alert('Tài khoản đã tồn tại');}</script>";
                    return RedirectToAction("AddEmployee", "Admin");
                }
            }
        }
        [NonAction]
        public bool CheckFormAddEmployee(string HoTen, string GioiTinh, string NgaySinh, string DiaChi, string Phone, string NgayVao, string ChucVu, string TaiKhooan, string MatKhau)
        {
            if (HoTen == null || GioiTinh == null || NgaySinh == null || DiaChi == null || Phone == null || NgayVao == null || ChucVu == null || TaiKhooan == null || MatKhau == null)
            {
                return false;
            }
            return true;
        }
        [NonAction]
        public async Task<bool> CheckEmployeeAccount_Database(string username)
        {
            if (await Database_ShopSport.EmployeeAccount.FirstOrDefaultAsync(x => x.TaiKhoan == username) != null)
                return true;
            return false;
        }





        //xóa employee
        [HttpGet]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {
            if (HttpContext.Session.GetInt32("ChucVu") != 0)
            {
                TempData["Message"] = "<script>window.onload = function () {alert('Bạn không có quyền xóa nhân viên');}</script>";
                return RedirectToAction("EmployeeManager", "Admin");
            }
            else
            {
                var emp =await Database_ShopSport.Employee.FirstOrDefaultAsync(x => x.EmployeeAccountId == id);
                var acc =await Database_ShopSport.EmployeeAccount.FirstOrDefaultAsync(x => x.EmployeeAccountId == id);
                Database_ShopSport.Employee.Remove(emp);
                await Database_ShopSport.SaveChangesAsync();
                Database_ShopSport.EmployeeAccount.Remove(acc);
                await Database_ShopSport.SaveChangesAsync();
                var cloudinary = new Cloudinary(new Account("dgtjdhrnq", "356379962535312", "1kgyT8lBh3odrCT0YV84tiIhKgE"));

                if (emp.Anh != "")
                {
                    var deletedinary = new DeletionParams(emp.EmployeeAccountId.ToString());
                    var result = await cloudinary.DestroyAsync(deletedinary);
                }
                TempData["Message"] = "<script>window.onload = function () {alert('Xóa thành công nhần viên');}</script>";
                return RedirectToAction("EmployeeManager", "Admin", new { page = 1 });
                
            }
        }

        //cập nhật nhân viên
        [HttpGet]
        public IActionResult EditEmployee([FromRoute] int id)
        {
            var emp = Database_ShopSport.Employee.FirstOrDefault(x => x.EmployeeAccountId == id);
            return View(emp);
        }
        [HttpPost]
        public async Task<IActionResult> EditEmployee(Emoloyee employeemodel,string GioiTinh,IFormFile file)
        {
            var employee = await Database_ShopSport.Employee.FirstOrDefaultAsync(x=>x.EmployeeId==employeemodel.EmployeeId);
            if(employee == null)
                return Ok("Lỗi");
            else
            {
                if(file==null)
                {
                    employee.HoTen = employeemodel.HoTen;
                    employee.NgayVao = employeemodel.NgayVao;
                    employee.NgaySinh = employeemodel.NgaySinh;
                    employee.GioiTinh = GioiTinh;
                    employee.DiaChi = employeemodel.DiaChi;
                    employee.Phone = employeemodel.Phone;
                    employee.ChucVu = employeemodel.ChucVu;
                }
                else
                {
                    var cloudinary = new Cloudinary(new Account("dgtjdhrnq", "356379962535312", "1kgyT8lBh3odrCT0YV84tiIhKgE"));

                    if(employee.Anh!="")
                    {
                        var deletedinary = new DeletionParams(employee.EmployeeAccountId.ToString());
                        var result = await cloudinary.DestroyAsync(deletedinary);
                    }    
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream()),
                        PublicId = employee.EmployeeAccountId.ToString(),
                    };
                    var uploadResult =await cloudinary.UploadAsync(uploadParams);
                    //Transformation
                    cloudinary.Api.UrlImgUp.Transform(new Transformation().Width(100).Height(150).Crop("fill")).BuildUrl("olympic_flag");
                    var imageUrl = uploadResult.SecureUrl.ToString();



                    employee.HoTen = employeemodel.HoTen;
                    employee.NgayVao = employeemodel.NgayVao;
                    employee.NgaySinh = employeemodel.NgaySinh;
                    employee.GioiTinh = GioiTinh;
                    employee.DiaChi = employeemodel.DiaChi;
                    employee.Phone = employeemodel.Phone;
                    employee.ChucVu = employeemodel.ChucVu;
                    employee.Anh = imageUrl;
                    //HttpContext.Session.SetString("UrlIamge", employee.Anh);
                }
                Database_ShopSport.Employee.Update(employee);
                await Database_ShopSport.SaveChangesAsync();
                TempData["Message"] = "<script>window.onload = function () {alert('Cập nhật thành công');}</script>";
                return RedirectToAction("EmployeeManager","Admin");
            }    
        }


        //hồ sơ nhân viên
        [HttpGet]
        public IActionResult Profile()
        {
            var epl=Database_ShopSport.Employee.FirstOrDefault(x=>x.EmployeeAccountId==HttpContext.Session.GetInt32("EmployyeeAccountId"));
            return View(epl);
        }
        [HttpGet]
        public IActionResult EditProfile()
        {
            var emp = Database_ShopSport.Employee.FirstOrDefault(x => x.EmployeeAccountId == HttpContext.Session.GetInt32("EmployyeeAccountId"));
            return View(emp);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(Emoloyee employeemodel, string GioiTinh, IFormFile file)
        {
            var employee =await Database_ShopSport.Employee.FirstOrDefaultAsync(x => x.EmployeeId == employeemodel.EmployeeId);
            if (employee == null)
                return Ok("Lỗi");
            else
            {
                if (file == null)
                {
                    employee.HoTen = employeemodel.HoTen;
                    employee.NgayVao = employeemodel.NgayVao;
                    employee.NgaySinh = employeemodel.NgaySinh;
                    employee.GioiTinh = GioiTinh;
                    employee.DiaChi = employeemodel.DiaChi;
                    employee.Phone = employeemodel.Phone;
                    employee.ChucVu = employee.ChucVu;
                }
                else
                {
                    var cloudinary = new Cloudinary(new Account("dgtjdhrnq", "356379962535312", "1kgyT8lBh3odrCT0YV84tiIhKgE"));
                    if (employee.Anh != "")
                    {
                        var deletedinary = new DeletionParams(employee.EmployeeAccountId.ToString());
                        var result = await cloudinary.DestroyAsync(deletedinary);
                    }
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream()),
                        PublicId = employee.EmployeeAccountId.ToString(),
                    };
                    var uploadResult = cloudinary.Upload(uploadParams);
                    //Transformation
                    cloudinary.Api.UrlImgUp.Transform(new Transformation().Width(100).Height(150).Crop("fill")).BuildUrl("olympic_flag");
                    var imageUrl = uploadResult.SecureUrl.ToString();



                    employee.HoTen = employeemodel.HoTen;
                    employee.NgayVao = employeemodel.NgayVao;
                    employee.NgaySinh = employeemodel.NgaySinh;
                    employee.GioiTinh = GioiTinh;
                    employee.DiaChi = employeemodel.DiaChi;
                    employee.Phone = employeemodel.Phone;
                    employee.ChucVu = employeemodel.ChucVu;
                    employee.Anh = imageUrl;
                    HttpContext.Session.SetString("UrlIamge", employee.Anh);
                }
                Database_ShopSport.Employee.Update(employee);
                await Database_ShopSport.SaveChangesAsync();
                TempData["Message"] = "<script>window.onload = function () {alert('Cập nhật thành công');}</script>";
                return RedirectToAction("Profile", "Admin");
            }
        }
        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DoiMatKhau(string MatKhauCu,string MatKhauMoi,string cfMatKhau)
        {
            if(!CheckEmtyDoiMatKhau(MatKhauCu,MatKhauMoi,cfMatKhau))
            {
                return View();
            }    
            else
            {
                var emp = Database_ShopSport.EmployeeAccount.FirstOrDefault(x => x.EmployeeAccountId == HttpContext.Session.GetInt32("EmployyeeAccountId"));
                if(emp.MatKhau!=Encrypt.ConvertToEncrypt(MatKhauCu))
                {
                    TempData["Message"] = "<script>window.onload = function () {alert('Mật khẩu hiện tại nhập không đúng');}</script>";
                    return View();
                }
                else
                {
                    emp.MatKhau = Encrypt.ConvertToEncrypt(MatKhauMoi);
                    Database_ShopSport.EmployeeAccount.Update(emp);
                    Database_ShopSport.SaveChanges();
                    TempData["Message"] = "<script>window.onload = function () {alert('Đổi mật khẩu thành công');}</script>";
                    return RedirectToAction("Index","Admin");
                }    
            }    
        }
        private bool CheckEmtyDoiMatKhau(string MatKhauCu, string MatKhauMoi, string cfMatKhau)
        {
            if(MatKhauCu==null || MatKhauMoi==null || cfMatKhau==null)
            {
                TempData["Message"] = "<script>window.onload = function () {alert('Vui lòng điền đầy đủ');}</script>";
                return false;
            }
            else if(MatKhauMoi!=cfMatKhau)
            {
                TempData["Message"] = "<script>window.onload = function () {alert('Mật khẩu mới nhập không giống nhau');}</script>";
                return false;
            }
            return true;
        }





        //DanhMuc
        [HttpGet]
        public async Task<IActionResult> CategoryManager()
        {
            var listCategory = await Database_ShopSport.Category.ToListAsync();
            return View(listCategory);
        }
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category categoryModel)
        {
            if(!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    TempData["Message"] = "<script>window.onload = function () {alert('" + error.ErrorMessage + "');}</script>";
                    break;
                }
                return View();
            }
            await Database_ShopSport.Category.AddAsync(categoryModel);
            await Database_ShopSport.SaveChangesAsync();
            TempData["Message"] = "<script>window.onload = function () {alert('Thêm thành công loại sản phẩm " +categoryModel.TenDanhMuc+"');}</script>";
            return RedirectToAction("CategoryManager","Admin");
        }
        [HttpGet]
        public async Task<IActionResult> EditCategory([FromRoute]int id)
        {
            var findCategory = await Database_ShopSport.Category.FirstOrDefaultAsync(x=>x.CategoryId==id);
            return View(findCategory);
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(Category categoryModel)
        {
            try
            {
                Database_ShopSport.Category.Update(categoryModel);
                await Database_ShopSport.SaveChangesAsync();
                TempData["Message"] = "<script>window.onload = function () {alert('Cập nhật thành công loại sản phẩm " + categoryModel.TenDanhMuc + "');}</script>";
                return RedirectToAction("CategoryManager","Admin");

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var findCategory = await Database_ShopSport.Category.FirstOrDefaultAsync(x => x.CategoryId == id);
            Database_ShopSport.Category.Remove(findCategory);
            await Database_ShopSport.SaveChangesAsync();
            TempData["Message"] = "<script>window.onload = function () {alert('Xóa thành công loại sản phẩm " + findCategory.TenDanhMuc + "');}</script>";
            return RedirectToAction("CategoryManager", "Admin");
        }
        //kết thúc danh mục




        //Sản phẩm
        public async  Task<IActionResult> ProductManager()
        {
            var ListProdcut = await Database_ShopSport.Products.ToListAsync();
            return View(ListProdcut);
        }
        [HttpGet]
        public JsonResult GetList()
        {
            var ListProdcut = Database_ShopSport.Products.ToList();
            return Json(ListProdcut);
        }
        [HttpGet]
        /*public async Task<IActionResult> EditHoatDongProDuct([FromRoute] int id)
        {
            var findProduct = await Database_ShopSport.Products.FirstOrDefaultAsync(x=>x.ProductId==id);
            if(findProduct.HoatDong==1)
                findProduct.HoatDong= 0;
            else
                findProduct.HoatDong = 1;
            Database_ShopSport.Products.Update(findProduct);
            await Database_ShopSport.SaveChangesAsync();
            return RedirectToAction("ProductManager", "Admin");
        }*/
        [HttpGet]
        public JsonResult EditHoatDongProDuct(int id)
        {
            var findProduct = Database_ShopSport.Products.FirstOrDefault(x => x.ProductId == id);
            if (findProduct.HoatDong == 1)
                findProduct.HoatDong = 0;
            else
                findProduct.HoatDong = 1;
            Database_ShopSport.Products.Update(findProduct);
            Database_ShopSport.SaveChanges();
            var ListProdcut = Database_ShopSport.Products.ToList();
            return Json(ListProdcut);
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            ViewData["CategoryId"] = new SelectList(Database_ShopSport.Category, "CategoryId", "TenDanhMuc");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product productModel, IFormFile file)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        TempData["Message"] = "<script>window.onload = function () {alert('" + error.ErrorMessage + "');}</script>";
                        break;
                    }
                    return RedirectToAction("Addproduct","Admin");
                }
                int LastProducId;
                if(await Database_ShopSport.Products.CountAsync()==0)
                    LastProducId = 1;
                else
                    LastProducId = await Database_ShopSport.Products.MaxAsync(x=>x.ProductId)+1;
                var cloudinary = new Cloudinary(new Account("dgtjdhrnq", "356379962535312", "1kgyT8lBh3odrCT0YV84tiIhKgE"));
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    PublicId = productModel.TenSanPham+ LastProducId.ToString(),
                };
                var uploadResult = cloudinary.Upload(uploadParams);
                //Transformation
                cloudinary.Api.UrlImgUp.Transform(new Transformation().Width(100).Height(150).Crop("fill")).BuildUrl("olympic_flag");
                var imageUrl = uploadResult.SecureUrl.ToString();



                productModel.Anh = imageUrl;
                await Database_ShopSport.AddAsync(productModel);
                await Database_ShopSport.SaveChangesAsync();
                TempData["Message"] = "<script>window.onload = function () {alert('Thêm thành công sản phẩm " + productModel.TenSanPham + "');}</script>";
                return RedirectToAction("ProductManager", "Admin");
                //return Ok("ok");
            }
            catch(Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditProduct([FromRoute] int id)
        {
            ViewData["CategoryId"] = new SelectList(Database_ShopSport.Category, "CategoryId", "TenDanhMuc");
            var findProduct = await Database_ShopSport.Products.FirstOrDefaultAsync(x=>x.ProductId==id);
            return View(findProduct);
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct( Product productModel, IFormFile file)
        {
            try
            {
                if(file!=null)
                {
                    var cloudinary = new Cloudinary(new Account("dgtjdhrnq", "356379962535312", "1kgyT8lBh3odrCT0YV84tiIhKgE"));
                    if (productModel.Anh != "")
                    {
                        var deletedinary = new DeletionParams(productModel.TenSanPham + productModel.ProductId.ToString());
                        var result = await cloudinary.DestroyAsync(deletedinary);
                    }
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, file.OpenReadStream()),
                        PublicId = productModel.TenSanPham + productModel.ProductId.ToString(),
                    };
                    var uploadResult = await cloudinary.UploadAsync(uploadParams);
                    //Transformation
                    cloudinary.Api.UrlImgUp.Transform(new Transformation().Width(100).Height(150).Crop("fill")).BuildUrl("olympic_flag");
                    var imageUrl = uploadResult.SecureUrl.ToString();

                    productModel.Anh = imageUrl;
                }     
                TempData["Message"] = "<script>window.onload = function () {alert('Cập nhật thành công sản phẩm " + productModel.TenSanPham + "');}</script>";
                Database_ShopSport.Products.Update(productModel);
                await Database_ShopSport.SaveChangesAsync();
                return RedirectToAction("ProductManager", "Admin");
            }
            catch(Exception ex)
            {
                TempData["Message"] = "<script>window.onload = function () {alert('"+ex.Message+"');}</script>";
                return View();
            }
        }
        /*[HttpGet]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            var findProduct = await Database_ShopSport.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            var cloudinary = new Cloudinary(new Account("dgtjdhrnq", "356379962535312", "1kgyT8lBh3odrCT0YV84tiIhKgE"));
            
            Database_ShopSport.Products.Remove(findProduct);
            await Database_ShopSport.SaveChangesAsync();
            TempData["Message"] = "<script>window.onload = function () {alert('Xóa sản phẩm " + findProduct.TenSanPham + " thành công');}</script>";
            return RedirectToAction("ProductManager", "Admin");
        }*/
        [HttpGet]
        public JsonResult DeleteProduct(int id)
        {
            var findProduct = Database_ShopSport.Products.FirstOrDefault(x => x.ProductId == id);
            var cloudinary = new Cloudinary(new Account("dgtjdhrnq", "356379962535312", "1kgyT8lBh3odrCT0YV84tiIhKgE"));

            Database_ShopSport.Products.Remove(findProduct);
            Database_ShopSport.SaveChanges();
            var ListProdcut = Database_ShopSport.Products.ToList();
            return Json(ListProdcut);
        }
        [HttpGet]
        public JsonResult SearchProduct(string text)
        {
            if(text=="" || text==null)
            {
                var ListProdcut = Database_ShopSport.Products.ToList();
                return Json(ListProdcut);
            }    
            if (int.TryParse(text,out int number))
            {
                var ListProdcut=Database_ShopSport.Products.Where(x=>x.ProductId==int.Parse(text) || x.TenSanPham.Contains(text) || x.Gia==int.Parse(text));
                return Json(ListProdcut);
            }
            else
            {
                var ListProdcut = Database_ShopSport.Products.Where(x =>x.TenSanPham.Contains(text) || x.TrangThai.Contains(text));
                return Json(ListProdcut);
            }    
        }








        //quản lý size
        [HttpGet]
        public IActionResult SizeManger()
        {
            return View();
        }
        public async Task<JsonResult> GetListSize()
        {
            var ListSize=await Database_ShopSport.Sizes.ToListAsync();
            return Json(ListSize);
        }
        [HttpPost]
        public async Task<IActionResult> AddSize([FromForm] string TenSize)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        TempData["Message"] = "<script>window.onload = function () {alert('" + error.ErrorMessage + "');}</script>";
                        break;
                    }
                    return RedirectToAction("SizeManger", "Admin");
                }
                else
                {
                    if(await CheckTenSizeInDataBase(TenSize))
                    {
                        TempData["Message"] = "<script>window.onload = function () {alert('Tên Size đã tồn tại');}</script>";
                        return RedirectToAction("SizeManger", "Admin");
                    }
                    else
                    {
                        var size = new Sport_Shop.Areas.Admin.Models.Size()
                        {
                            TenSize= TenSize
                        };
                        await Database_ShopSport.Sizes.AddAsync(size);
                        await Database_ShopSport.SaveChangesAsync();
                        TempData["Message"] = "<script>window.onload = function () {alert('Thêm thành công');}</script>";
                        return RedirectToAction("SizeManger", "Admin");
                    }
                }    

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [NonAction]// kiểm tra tên size trong database có chưa. nếu có là true và ngược lại
        public async Task<bool> CheckTenSizeInDataBase(string tensize)
        {
            var findsize = await Database_ShopSport.Sizes.FirstOrDefaultAsync(x => x.TenSize == tensize);
            if(findsize!=null)
            {
                return true;
            }
            return false;
        }
        [HttpGet]
        public async Task<JsonResult> DeleteSize(string sizename)
        {
            var findsize= await Database_ShopSport.Sizes.FirstOrDefaultAsync(x=>x.TenSize==sizename);
            Database_ShopSport.Sizes.Remove(findsize);
            await Database_ShopSport.SaveChangesAsync();

            var listsize=await Database_ShopSport.Sizes.ToListAsync();
            return Json(listsize);
        }
        [HttpGet]
        public async Task<JsonResult> EditSize(int id)
        {
            var findsize=await Database_ShopSport.Sizes.FirstOrDefaultAsync(i=>i.SizeId==id);
            return Json(findsize);
        }
        public async Task<IActionResult> UpdateSize([FromRoute] int id, [FromForm] string TenSize)
        {
            var findsize=await Database_ShopSport.Sizes.FirstOrDefaultAsync(i=>i.SizeId==id);
            findsize.TenSize = TenSize;
            Database_ShopSport.Sizes.Update(findsize);
            await Database_ShopSport.SaveChangesAsync();    
            TempData["Message"] = "<script>window.onload = function () {alert('Cập nhật thành công size "+ TenSize + "');}</script>";
            return RedirectToAction("SizeManger", "Admin");
        }









        //Quản lý size sản phẩm
        public async Task<IActionResult> ProductSizeManager()
        {
            ViewData["ProductId"] = new SelectList(Database_ShopSport.Products, "ProductId", "TenSanPham");
            ViewData["SizeId"] = new SelectList(Database_ShopSport.Sizes, "SizeId", "TenSize");
            return View();
        }
        public async Task<JsonResult> LoadProDuctSize()
        {
            var listproductSize = from e in Database_ShopSport.productSizes
                                          join d in Database_ShopSport.Products on e.ProductId equals d.ProductId
                                          join f in Database_ShopSport.Sizes on e.SizeId equals f.SizeId
                                          select new
                                          {
                                              Id = e.ProductSizeId,
                                              TenSanPham = d.TenSanPham,
                                              Size = f.TenSize,
                                              SoLuongTon = e.SoLuongTon,
                                          };
            return Json(listproductSize);
        }
        [HttpPost]
        public async Task<IActionResult> AddProductSize(ProductSize model)
        {
            try
            {
                if(! await CheckProductSize(model.ProductId,model.SizeId))
                {
                    await Database_ShopSport.AddAsync(model);
                    await Database_ShopSport.SaveChangesAsync();
                    TempData["Message"] = "<script>window.onload = function () {alert('Thêm thành công ');}</script>";
                    return RedirectToAction("ProductSizeManager", "Admin");
                }    
                else
                {
                    TempData["Message"] = "<script>window.onload = function () {alert('Đã có. vui lòng kiểm tra lại');}</script>";
                    return RedirectToAction("ProductSizeManager", "Admin");
                }    
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        [NonAction]
        public async Task<bool> CheckProductSize(int proudctId,int sizeid)
        {
            var find = await Database_ShopSport.productSizes.FirstOrDefaultAsync(i => i.ProductId == proudctId && i.SizeId == sizeid);
            if (find == null)
                return false;
            return true;
        }
        [HttpGet]
        public async Task<JsonResult> EditProuctSize(int id)
        {
            var findproductsize = from e in Database_ShopSport.productSizes
                                  join d in Database_ShopSport.Products on e.ProductId equals d.ProductId
                                  join f in Database_ShopSport.Sizes on e.SizeId equals f.SizeId
                                  where e.ProductSizeId==id
                                  select new
                                  {
                                      Id = e.ProductSizeId,
                                      TenSanPham = d.TenSanPham,
                                      TenSize = f.TenSize,
                                  };
            return Json(findproductsize);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProductSize([FromRoute] int id,[FromForm] int SoLuongTon)
        {
            try
            {
                var find = await Database_ShopSport.productSizes.FirstOrDefaultAsync(i => i.ProductSizeId == id);
                find.SoLuongTon += SoLuongTon;
                Database_ShopSport.productSizes.Update(find);
                await Database_ShopSport.SaveChangesAsync();
                TempData["Message"] = "<script>window.onload = function () {alert('Thêm số lượng tồn thành công');}</script>";
                return RedirectToAction("ProductSizeManager", "Admin");
            }
            catch(Exception ex)
            {
                return Ok(ex.InnerException.Message);
            }




            //return Ok(id+"   "+TenSanPham +" "+TenSize + " "+SoLuongTon+"" );
        }
        [HttpGet]
        public async Task<IActionResult> DeleteProductSize([FromRoute] int id)
        {
            var find = await Database_ShopSport.productSizes.FirstOrDefaultAsync(x=>x.ProductSizeId==id);
            Database_ShopSport.productSizes.Remove(find);
            await Database_ShopSport.SaveChangesAsync();
            return RedirectToAction("ProductSizeManager","Admin");
        }    


        [HttpGet]
        public async Task<IActionResult> DonHang()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ListDonHang()
        {
            var result = Database_ShopSport.orders.OrderByDescending(data => data.OrderId).ToList();
            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> SearchDonHangByTrangThai(int trangthai)
        {
            var list=await Database_ShopSport.orders.Where(x=>x.TrangThai== trangthai).OrderByDescending(data => data.OrderId).ToListAsync();
            return Json(list);
        }
        [HttpGet]
        public async Task<IActionResult> SearchDonHang(string text)
        {
            if (text == "" || text == null)
            {
                var List =await Database_ShopSport.orders.OrderByDescending(data => data.OrderId).ToListAsync();
                return Json(List);
            }
            if (int.TryParse(text, out int number))
            {
                var List = Database_ShopSport.orders.Where(x => x.OrderId == int.Parse(text) || x.NguoiNhan.Contains(text) || x.DiaChi.Contains(text) || x.Phone==text);
                return Json(List);
            }
            else
            {
                var List = Database_ShopSport.orders.Where(x=> x.NguoiNhan.Contains(text) || x.DiaChi.Contains(text) || x.Phone == text);
                return Json(List);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditTrangThaiDonHang(int madh,int text)
        {
            var findh = await Database_ShopSport.orders.FirstOrDefaultAsync(x => x.OrderId == madh);
            findh.TrangThai = text;
            Database_ShopSport.orders.Update(findh);
            await Database_ShopSport.SaveChangesAsync();

            var List = await Database_ShopSport.orders.OrderByDescending(data => data.OrderId).ToListAsync();


            if (text == 3)
            {
                var orderItems = await Database_ShopSport.orderDetails.Where(o => o.OrderId == madh).ToListAsync();
                foreach (var item in orderItems)
                {
                    var findProductSize = await Database_ShopSport.productSizes.FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.SizeId == item.SizeId);
                    findProductSize.SoLuongTon -= item.SoLuong;
                    Database_ShopSport.productSizes.Update(findProductSize);
                    await Database_ShopSport.SaveChangesAsync();
                }
            }
            return new JsonResult(new
            {
                redirectToUrl = Url.Action("DonHang", "Admin")
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetOrderDetailByOrderId(int madh)
        {
            var List=from e in Database_ShopSport.orderDetails
                    join d in Database_ShopSport.Products on e.ProductId equals d.ProductId
                    join f in Database_ShopSport.Sizes on e.SizeId equals f.SizeId
                    where e.OrderId == madh 
                    select new
                    {
                        TenSanPham = d.TenSanPham,
                        TenSize = f.TenSize,
                        Anh=d.Anh,
                        SoLuong=e.SoLuong,
                        Gia=e.Gia,
                    };
            return Json(List);
        }
    }
}
