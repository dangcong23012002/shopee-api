using Project.Models;

public class DataViewModel
{
    public Status Status { get; set; }
    public IEnumerable<User> Users { get; set; }
    public IEnumerable<User> User { get; set; }
    public IEnumerable<UserInfo> UserInfos { get; set; }
    public IEnumerable<UserInfo> UserInfo { get; set; }
    public IEnumerable<Seller> Sellers { get; set; }
    public IEnumerable<Seller> Seller { get; set; }
    public IEnumerable<Store> Store { get; set; }
    public IEnumerable<SellerInfo> SellerInfo { get; set; }
    public IEnumerable<CartDetail> CartDetails { get; set; }
    public IEnumerable<Order> Orders { get; set; }
    public IEnumerable<OrderDetail> OrderDetails { get; set; }
    public int RoleID { get; set; }
    public int UserID { get; set; }
    public string Username { get; set; }
    public int CartCount { get; set; }
}