using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sport_Shop.Areas.Admin.Models
{
    [Table("Employee")]
    public class Emoloyee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }
        [Column(TypeName = "NVARCHAR(60)")]
        public string  HoTen { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        [Column(TypeName = "NVARCHAR(200)")]
        public string DiaChi { get; set; }
        [Column(TypeName = "VARCHAR(15)")]
        public string Phone { get; set; }
        public DateTime NgayVao { get; set; }
        public int ChucVu { get; set; }
        [Column(TypeName = "VARCHAR(200)")]
        public string Anh { get; set; }
        public int EmployeeAccountId { get; set; }
        [ForeignKey("EmployeeAccountId")]
        public virtual EmployeeAccount EmployeeAccount{ get; set; }
    }
}
