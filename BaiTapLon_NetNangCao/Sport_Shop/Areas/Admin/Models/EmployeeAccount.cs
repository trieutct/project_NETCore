using MessagePack;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sport_Shop.Areas.Admin.Models
{
    [Table("EmployeeAccount")]
    public class EmployeeAccount
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeAccountId { get; set; }
        [Column(TypeName ="VARCHAR(40)")]
        public string TaiKhoan { get; set; }
        [Column(TypeName = "VARCHAR(100)")]
        public string MatKhau { get; set; }
        public virtual Emoloyee Emoloyee { get; set; }

    }
}
