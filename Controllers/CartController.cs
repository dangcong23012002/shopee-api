using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Project.Models;

public class CartController : Controller {
    private readonly IHttpContextAccessor _accessor;
    private readonly DatabaseContext _context;
    private readonly IHomeResponsitory _homeResponsitory;
    private readonly ICartReponsitory _cartResponsitory;
    private readonly IUserResponsitory _userResponsitory;
    public CartController(IHttpContextAccessor accessor, DatabaseContext context, ICartReponsitory cartReponsitoty, IUserResponsitory userResponsitory, IHomeResponsitory homeResponsitory)
    {
        _accessor = accessor;
        _context = context;
        _homeResponsitory = homeResponsitory;
        _cartResponsitory = cartReponsitoty;
        _userResponsitory = userResponsitory;
    }

    [Route("cart")]
    [HttpGet]
    public IActionResult Index() {
        // Lấy Cookies trên trình duyệt
        var userID = Request.Cookies["UserID"];
        if (userID != null)
        {
            _accessor?.HttpContext?.Session.SetInt32("UserID", Convert.ToInt32(userID));
        } else {
            return View();
        }
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        if (sessionUserID != null)
        {
            List<User> users = _userResponsitory.checkUserLogin(Convert.ToInt32(sessionUserID)).ToList();
            _accessor?.HttpContext?.Session.SetString("UserName", users[0].sUserName);
            _accessor?.HttpContext?.Session.SetInt32("RoleID", users[0].FK_iRoleID);
        }
        else
        {
            _accessor?.HttpContext?.Session.SetString("UserName", "");
        }
        System.Console.WriteLine("UserID: " + userID);
        if (sessionUserID == 0) {
            return Redirect("/user/login");
        }
        IEnumerable<Store> stores = _homeResponsitory.getStores();
        IEnumerable<CartDetail> carts = _cartResponsitory.getCartInfo(Convert.ToInt32(sessionUserID)); 
        // Lấy số lượng giỏ hàng
        int cartCount = carts.Count();
        ShopeeViewModel model = new ShopeeViewModel {
            Stores = stores,
            CartDetails = carts,
            CartCount = cartCount
        };
        return View(model); 
    }

    [HttpPost]
    [Route("/cart")]
    public IActionResult GetCartInfo() {
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        var sessionUsername = _accessor?.HttpContext?.Session.GetString("UserName");  
        var sessionRoleID = _accessor?.HttpContext?.Session.GetInt32("RoleID");
        IEnumerable<CartDetail> carts = _cartResponsitory.getCartInfo(Convert.ToInt32(sessionUserID)); 
        IEnumerable<Product> get12ProductsAndSortAsc = _cartResponsitory.get12ProductsAndSortAsc(); 
        int cartCount = carts.Count();
        CartViewModel model = new CartViewModel {
            CartDetails = carts,
            Get12ProductsAndSortAsc = get12ProductsAndSortAsc,
            CartCount = cartCount,
            RoleID = Convert.ToInt32(sessionRoleID),
            UserID = Convert.ToInt32(sessionUserID),
            Username = sessionUsername
        };
        return Json(model);  
    }

    [HttpPost]
    public IActionResult AddToCart(int productID, double unitPrice, int quantity)
    {
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        if (sessionUserID == null) {
            sessionUserID = 0;
        } 
        
        List<User> user = _userResponsitory.checkUserLogin(Convert.ToInt32(sessionUserID)).ToList();

        List<CartDetail> checkProduct = _cartResponsitory.checkProduct(Convert.ToInt32(sessionUserID), productID).ToList();

        if (user.Count() == 0)
        {
            string msg = "Bạn phải đăng nhập mới được thêm vào giỏ hàng!";
            return Json(new { msg });
        } else if (checkProduct.Count() != 0) // Kiểm tra sản phẩm bị trùng trong giỏ hàng
        {
            string msg = "Sản phẩm này đã có trong giỏ hàng";
            return Json(new {msg});
        }
        else
        {
            // https://www.c-sharpcorner.com/blogs/date-and-time-format-in-c-sharp-programming1
            // Thêm mã giỏ hàng
            SqlParameter updateTimeParam = new SqlParameter("@dUpdateTime", DateTime.Now.ToString("dd/MM/yyyy"));
            List<Cart> cart = _cartResponsitory.checkCartIDExist().ToList();

            int cartID;
            if (cart.Count() != 0) {
                cartID = cart[0].PK_iCartID;
                var updateTime = cart[0].dUpdateTime;
            } else {
                _context.Database.ExecuteSqlRaw("SET DATEFORMAT dmy EXEC sp_InsertCart @dUpdateTime", updateTimeParam);
                List<Cart> newCart = _context.Carts.FromSqlRaw("SET DATEFORMAT dmy EXEC sp_GetCartIDByTime @dUpdateTime", updateTimeParam).ToList();
                cartID = newCart[0].PK_iCartID;
            }
            // Thêm vào chi tiết giỏ hàng
            _cartResponsitory.insertCartDetail(Convert.ToInt32(sessionUserID), productID, cartID, quantity, unitPrice);
            IEnumerable<CartDetail> carts = _cartResponsitory.getCartInfo(Convert.ToInt32(sessionUserID));
            IEnumerable<CartDetail> cartDetails = _cartResponsitory.getCartInfo(Convert.ToInt32(sessionUserID)).ToList();
            int cartCount = carts.Count();
            ProductViewModel model = new ProductViewModel {
                CartCount = cartCount,
                CartDetails = cartDetails
            };
            string msg = "Thêm vào giỏ hàng thành công!";
            return Json(new { msg, model });
        }
    }

    // Hàm lấy số lượng sản phẩm trong giỏ hàng
    [HttpPost]
    [Route("/cart/quantity")]
    public IActionResult Quantity(int productID, int quantity, double unitPrice) {
        double money = quantity * unitPrice;
        _cartResponsitory.changeQuantity(Convert.ToInt32(_accessor?.HttpContext?.Session.GetInt32("UserID")), productID, quantity, money);
        return Json(new {money});
    }

    [HttpPost]
    [Route("/cart/delete-item")]
    public IActionResult DeleteItem(int productID) {
        _cartResponsitory.deleteProductInCart(productID, Convert.ToInt32(_accessor?.HttpContext?.Session.GetInt32("UserID")));
        string msg = "Sản phẩm đã được xoá khỏi giỏ hàng!";
        IEnumerable<CartDetail> cartDetails = _cartResponsitory.getCartInfo(Convert.ToInt32(_accessor?.HttpContext?.Session.GetInt32("UserID")));
        int cartCount = cartDetails.Count();
        CartViewModel model = new CartViewModel {
            CartCount = cartCount,
            Message = msg
        };
        return Ok(model);
    }

    [HttpPost]
    public IActionResult DeleteAllProduct() {
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        List<CartDetail> cartDetails = _cartResponsitory.getCartInfo(Convert.ToInt32(sessionUserID)).ToList(); // Phải sử dụng list thì mới lấy ra được các id
        foreach (var item in cartDetails) {
            _cartResponsitory.deleteProductInCart(item.PK_iProductID, Convert.ToInt32(sessionUserID));
        }
        
        CartViewModel model = new CartViewModel {
            CartCount = cartDetails.Count(),
            Message = "Xoá thành công"
        };
        return Ok(model);
    }
}