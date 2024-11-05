using System.ComponentModel.DataAnnotations;

public class ForgotViewModel
{
    [Required(ErrorMessage = "Email không được trống!")]
    public string sEmail { get; set; }
}