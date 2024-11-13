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

    [Route("/cart/{userID?}")]
    [HttpGet]
    public IActionResult Index(int userID = 0) {
        List<User> user = _userResponsitory.checkUserLogin(userID).ToList();
        IEnumerable<CartDetail> carts = _cartResponsitory.getCartInfo(userID); 
        IEnumerable<Product> get12ProductsAndSortAsc = _cartResponsitory.get12ProductsAndSortAsc(); 
        int cartCount = carts.Count();
        CartViewModel model = new CartViewModel {
            CartDetails = carts,
            Get12ProductsAndSortAsc = get12ProductsAndSortAsc,
            CartCount = cartCount,
            RoleID = user[0].FK_iRoleID,
            UserID = userID,
            Username = user[0].sUserName
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/cart/add-to-cart")]
    public IActionResult AddToCart(int userID, int productID, double unitPrice, int quantity)
    {
        List<User> user = _userResponsitory.checkUserLogin(userID).ToList();
        List<CartDetail> checkProduct = _cartResponsitory.checkProduct(userID, productID).ToList();
        Status status;
        if (user.Count() == 0)
        {
            status = new Status {
                StatusCode = -1,
                Message = "Bạn phải đăng nhập mới được thêm vào giỏ hàng!"
            };
        } else if (checkProduct.Count() != 0) // Kiểm tra sản phẩm bị trùng trong giỏ hàng
        {
            quantity = checkProduct[0].iQuantity + quantity;
            double money = quantity * unitPrice;
            _cartResponsitory.changeQuantity(userID, productID, quantity, money);
            status = new Status {
                StatusCode = 0,
                Message = "Thêm sản phẩm thành công!"
            };
        }
        else
        {
            // https://www.c-sharpcorner.com/blogs/date-and-time-format-in-c-sharp-programming1
            // Thêm mã giỏ hàng
            List<Cart> cart = _cartResponsitory.checkCartIDExist().ToList();
            int cartID;
            if (cart.Count() != 0) {
                cartID = cart[0].PK_iCartID;
            } else {
                _cartResponsitory.insertCart();
                List<Cart> newCart = _cartResponsitory.getCartIDByTime().ToList();
                cartID = newCart[0].PK_iCartID;
            }
            // Thêm vào chi tiết giỏ hàng
            _cartResponsitory.insertCartDetail(userID, productID, cartID, quantity, unitPrice);
            status = new Status {
                StatusCode = 1,
                Message = "Thêm vào giỏ hàng thành công!"
            };
        }
        IEnumerable<CartDetail> cartDetails = _cartResponsitory.getCartInfo(userID).ToList();
        int cartCount = cartDetails.Count();
        ProductViewModel model = new ProductViewModel
        {
            Status = status,
            CartCount = cartCount,
            CartDetails = cartDetails
        };
        return Ok(model);
    }

    // Cập nhật số lượng sản phẩm trong giỏ hàng
    [HttpPost]
    [Route("/cart/quantity")]
    public IActionResult Quantity(int userID, int productID, int quantity, double unitPrice) {
        List<User> user = _userResponsitory.checkUserLogin(userID).ToList();
        Status status;
        if (user.Count() == 0)
        {
            status = new Status {
                StatusCode = -1,
                Message = "Bạn phải đăng nhập mới được thêm vào giỏ hàng!"
            };
        } else {
            double money = quantity * unitPrice;
            _cartResponsitory.changeQuantity(userID, productID, quantity, money);
            status = new Status {
                StatusCode = 1,
                Message = "Thành tiền là: " + money
            };
        }
        DataViewModel model = new DataViewModel {
            Status = status
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/cart/delete-item")]
    public IActionResult DeleteItem(int userID, int productID) {
        Status status;
        if (_cartResponsitory.deleteProductInCart(productID, userID)) {
            status = new Status {
                StatusCode = 1,
                Message = "Sản phẩm đã được xoá khỏi giỏ hàng!"
            };
        } else {
            status = new Status {
                StatusCode = 1,
                Message = "Xoá sản phẩm thất bại!"
            };
        }
        IEnumerable<CartDetail> cartDetails = _cartResponsitory.getCartInfo(userID);
        int cartCount = cartDetails.Count();
        DataViewModel model = new DataViewModel {
            CartDetails = cartDetails,
            CartCount = cartCount,
            Status = status
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/cart/delete-item-all")]
    public IActionResult DeleteAllProduct(int userID) {
        List<CartDetail> cartDetails = _cartResponsitory.getCartInfo(userID).ToList(); // Phải sử dụng list thì mới lấy ra được các id
        foreach (var item in cartDetails) {
            _cartResponsitory.deleteProductInCart(item.PK_iProductID, userID);
        }
        Status status = new Status {
            StatusCode = 1,
            Message = "Xoá thành công!"
        };
        DataViewModel model = new DataViewModel {
            CartDetails = cartDetails,
            CartCount = cartDetails.Count(),
            Status = status
        };
        return Ok(model);
    }
}