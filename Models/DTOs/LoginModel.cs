using System.ComponentModel.DataAnnotations;

public class LoginModel {
    /// <summary>
    /// DTO hay tên gọi đầy đủ là Data Tranfer Object 
    /// là một design pattern lần đầu tiên được giới thiệu bởi Martin Fowler trong cuốn sách EAA. 
    /// Mục đích sử dụng chính của DTO đó là giảm số lần gọi các method giữa các tiến trình xử lý.
    /// Nguồn: https://shareprogramming.net/dto-la-gi-dung-dto-trong-nhung-truong-hop-nao/
    /// </summary>
    [Required(ErrorMessage = "Email không được trống!")]
    public string sEmail { get; set; }
    [Required(ErrorMessage = "Mật khẩu không được trống")]
    //[RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Mật khẩu phải chứa chữ hoa, chữ thường, số và ký tự")]
    public string sPassword { get; set; }
}