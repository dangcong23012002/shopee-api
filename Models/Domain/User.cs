using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models
{
    public class User
    {
        public int PK_iUserID { get; set; }
        public int FK_iRoleID { get; set; }
        [Required(ErrorMessage = "Bạn chưa nhập tên!")]
        public string sUserName { get; set; }
        [Required(ErrorMessage = "Bạn chưa nhập email")]
        public string sEmail { get; set; }
        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu")]
        public string sPassword { get; set; }
        public DateTime dCreateTime { get; set; }
        public string sRoleName { get; set; }
        public string sRoleDescription { get; set; }
    }
}
