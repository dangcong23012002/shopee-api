public class ShopViewModel
{
    public IEnumerable<Store> Stores { get; set; }
    public IEnumerable<MakeNotice> MakeNotices { get; set; }
    public IEnumerable<SliderShop> SlidersShop { get; set; }
    public IEnumerable<Category> Categories { get; set;}
    public IEnumerable<Product> Products { get; set; }
    public IEnumerable<CartDetail> CartDetails { get; set; }
    public IEnumerable<Product> Top3SellingProducts { get; set; }
    public IEnumerable<Product> Top10SellingProducts { get; set; }
    public IEnumerable<Product> Top10GoodPriceProducts { get; set; }
    public IEnumerable<Product> Top10SuggestProducts { get; set; }
    public int TotalPage { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int RoleID { get; set; }
    public int UserID { get; set; }
    public string Username { get; set; }
    public int CartCount { get; set; }
    public int CurrentCategoryID { get; set; }
}