using System.ComponentModel.DataAnnotations;
using Project.Models;

public class UserViewModel
{
    public int UserID { get; set; }
    public IEnumerable<User> Users { get; set; }
}