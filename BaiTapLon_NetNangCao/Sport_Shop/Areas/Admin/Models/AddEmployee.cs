using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sport_Shop.Areas.Admin.Models
{
    public class AddEmployee
    {
        public int EmployeeId { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string Phone { get; set; }
        public DateTime NgayVao { get; set; }
        public int ChucVu { get; set; }
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
    }
}
