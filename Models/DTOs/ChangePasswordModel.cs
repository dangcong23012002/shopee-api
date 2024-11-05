using System.ComponentModel.DataAnnotations;

public class ChangePasswordModel
{
    [Required(ErrorMessage = "Mật khẩu cũ không được trống!")]
    public string sOldPassword { get; set; }
    [Required(ErrorMessage = "Mật khẩu mới không được trống!")]
    public string sNewPassword { get; set; }
    [Required(ErrorMessage = "Mật khẩu không được trống!")]
    [Compare("sNewPassword", ErrorMessage = "Hai mật khẩu phải giống nhau!")]
    public string sReNewPassword { get; set; }
}