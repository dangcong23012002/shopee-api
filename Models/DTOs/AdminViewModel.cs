public class AdminViewModel
{
    public IEnumerable<Order> OrdersWaitSettlment { get; set; }
    public IEnumerable<Order> OrdersWaitPickup { get; set; }
    public IEnumerable<Order> OrdersPicking { get; set; }
    public IEnumerable<Order> OrdersDelivering { get; set; }
    public IEnumerable<Order> OrdersCompleted { get; set; }
    public IEnumerable<SellerInfo> SellerInfos { get; set; }
    public IEnumerable<ShippingOrder> ShippingOrders { get; set; }
    public string HtmlWaitSettlmentItem { get; set; }
    public string HtmlPickingItem { get; set; }
    public string HtmlUsersInfoItem { get; set; }
    public string HtmlDeliveringItem { get; set; }
    public IEnumerable<OrderDetail> OrderDetails { get; set; }
    public IEnumerable<OrderDetail> OrderDetailsPickingUp { get; set; }
    public List<Address> Addresses { get; set; }
    public List<Payment> Payments { get; set; }
    public IEnumerable<Industry> Industries { get; set; }
    public IEnumerable<Industry> Industry { get; set; }
    public IEnumerable<ParentCategory> ParentCategories { get; set; }
    public IEnumerable<CategoryModel> Categories { get; set; }
    public IEnumerable<CategoryModel> Category { get; set; }
    public IEnumerable<UserInfo> UserInfos { get; set; }
    public int RoleID { get; set; }
    public int UserID { get; set; }
    public string Username { get; set; }
    public Status Status { get; set; }
}