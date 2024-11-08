using Microsoft.AspNetCore.Mvc;
using Project.Models;

public class ShopController : Controller
{
    private readonly IHttpContextAccessor _accessor;
        private readonly IHomeResponsitory _homeResponsitory;
        private readonly IShopResponsitory _shopResponsitory;
        private readonly ICartReponsitory _cartResponsitory;
        private readonly IUserResponsitory _userResponsitory;
        private readonly IProductResponsitory _productResponsitory;
        private readonly IChatRepository _chatRepository;

        public ShopController(
            IHttpContextAccessor accessor, 
            IHomeResponsitory homeResponsitory, 
            ICartReponsitory cartReponsitory, 
            IUserResponsitory userResponsitory, 
            IShopResponsitory shopResponsitory,
            IProductResponsitory productResponsitory,
            IChatRepository chatRepository
            )
        {
            _accessor = accessor;
            _homeResponsitory = homeResponsitory;
            _cartResponsitory = cartReponsitory;
            _userResponsitory = userResponsitory;
            _shopResponsitory = shopResponsitory;
            _productResponsitory = productResponsitory;
            _chatRepository = chatRepository;
        }

    [HttpGet]
    [Route("/shop/{shopUsername?}/{userID?}")]
    public IActionResult Index(string shopUsername = "", int currentPage = 1, int userID = 0, int categoryID = 0) {
        List<User> user = _userResponsitory.checkUserLogin(userID).ToList();
        List<Store> shop = _shopResponsitory.getShopByUsername(shopUsername).ToList();
        IEnumerable<MakeFriend> makeFriends = _chatRepository.getMakeFriendByUserIDAndShopID(userID, shop[0].PK_iStoreID);
        IEnumerable<Product> products;
        IEnumerable<CartDetail> cartDetails = _cartResponsitory.getCartInfo(Convert.ToInt32(userID));
        IEnumerable<SliderShop> slidersShop = _shopResponsitory.getSlidersShopByShopID(shop[0].PK_iStoreID);
        IEnumerable<Category> categories = _shopResponsitory.getCategoriesByShopID(shop[0].PK_iStoreID);
        if (categoryID != 0) {
            products = _productResponsitory.getProductsByCategoryID(categoryID);
        } else {
            products = _shopResponsitory.getProductsByShopID(shop[0].PK_iStoreID);
        }
        IEnumerable<Product> top3SellingProducts = _shopResponsitory.getTop3SellingProductsShop(shop[0].PK_iStoreID);
        IEnumerable<Product> top10SellingProducts = _shopResponsitory.getTop10SellingProductsShop(shop[0].PK_iStoreID);
        IEnumerable<Product> top10GoodPriceProducts = _shopResponsitory.getTop10GoodPriceProductsShop(shop[0].PK_iStoreID);
        IEnumerable<Product> top10SuggestProducts = _shopResponsitory.getTop10SuggestProductsShop(shop[0].PK_iStoreID);
        int totalRecord = products.Count();
        int pageSize = 10;
        int totalPage = (int) Math.Ceiling(totalRecord / (double) pageSize);
        products = products.Skip((currentPage - 1) * pageSize).Take(pageSize);
        ShopViewModel model = new ShopViewModel {
            Stores = shop,
            MakeFriends = makeFriends,
            SlidersShop = slidersShop,
            Categories = categories,
            Products = products,
            Top3SellingProducts = top3SellingProducts,
            Top10SellingProducts = top10SellingProducts,
            Top10GoodPriceProducts = top10GoodPriceProducts,
            Top10SuggestProducts = top10SuggestProducts,
            TotalPage = totalPage,
            PageSize = pageSize,
            CurrentPage = currentPage,
            RoleID = user[0].FK_iRoleID,
            UserID = user[0].PK_iUserID,
            Username = user[0].sUserName,
            CartDetails = cartDetails,
            CartCount = cartDetails.Count(),
            CurrentCategoryID = categoryID
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/shop/send-make-friend")]
    public IActionResult SendFriend(int sellerID = 0) {
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        if (sessionUserID == null) {
            sessionUserID = 0;
        } 
        List<User> user = _userResponsitory.checkUserLogin(Convert.ToInt32(sessionUserID)).ToList();
        Status status;
        if (user.Count() == 0) {
            status = new Status {
                StatusCode = -1,
                Message = "Bạn phải đăng nhập thì mới kết bạn được!"
            };
        } else {
            _chatRepository.insertMakeNoice(Convert.ToInt32(sessionUserID), sellerID);
            status = new Status {
                StatusCode = 1,
                Message = "Gửi lời kết bạn thành công!"
            };
        }
        ShopeeViewModel model = new ShopeeViewModel {
            Status = status
        };
        return Ok(model);
    }
}