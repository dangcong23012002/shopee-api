using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Project.Models;

public class OrderController : Controller {
    private readonly DatabaseContext _context;
    private readonly IOrderResponsitory _orderResponsitory;
    private readonly ICartReponsitory _cartReponsitory;
    private readonly IProductResponsitory _productResponsitory;
    private readonly IHttpContextAccessor _accessor;
    public OrderController(DatabaseContext context, IHttpContextAccessor accessor, IOrderResponsitory orderResponsitory, ICartReponsitory cartReponsitory, IProductResponsitory productResponsitory)
    {
        _context = context;
        _accessor = accessor;
        _orderResponsitory = orderResponsitory;
        _cartReponsitory = cartReponsitory;
        _productResponsitory = productResponsitory;
    }

    public IActionResult Index() {
        return View();
    }

    List<Checkout> checkouts => HttpContext.Session.Get<List<Checkout>>("cart_key") ?? new List<Checkout>();
    [HttpPost]
    public IActionResult Checkout() {
        var cartsCheckout = checkouts;
        // Fix cứng cũng phải khai báo SqlParameter
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        List<CartDetail> carts = _cartReponsitory.getCartInfo(Convert.ToInt32(sessionUserID)).ToList();
        var totalMoney = _orderResponsitory.totalMoneyProductInCart(Convert.ToInt32(sessionUserID));
        foreach (var checkout in carts) {
            var item = cartsCheckout.SingleOrDefault(p => p.PK_iProductID == checkout.PK_iProductID);
            if (item == null) {
                List<Product> product = _productResponsitory.getProductByID(checkout.PK_iProductID).ToList();
                item = new Checkout {
                    PK_iProductID = product[0].PK_iProductID,
                    sProductName = product[0].sProductName,
                    sImageUrl = product[0].sImageUrl,
                    dUnitPrice = product[0].dPrice,
                    iQuantity = checkout.iQuantity,
                    dMoney = product[0].dPrice * checkout.iQuantity
                };
                cartsCheckout.Add(item);
            }
        }
        HttpContext.Session.Set("cart_key", cartsCheckout);
        OrderViewModel model = new OrderViewModel {
            TotalMoney = totalMoney,
            CartCount = carts.Count(),
            Checkouts = checkouts
        };
        return Json(model); 
    }

    [HttpPost]
    [Route("/order/confirm-deliverd")]
    public IActionResult Delivered(int orderID = 0) {
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        Status status;
        if (_orderResponsitory.confirmOrderAboutReceived(orderID)) {
            status = new Status {
                StatusCode = 1,
                Message = "Xác nhận thành công"
            };
        } else {
            status = new Status {
                StatusCode = -1,
                Message = "Xác nhận thất bại"
            };
        }
        IEnumerable<Order> ordersDelivered = _orderResponsitory.getOrderByUserIDDeliverd(Convert.ToInt32(sessionUserID));
        IEnumerable<OrderDetail> orderDetailsDelivered = _orderResponsitory.getProductsOrderByUserIDDelivered(Convert.ToInt32(sessionUserID));
        OrderViewModel model = new OrderViewModel {
            Status = status,
            OrdersDelivered = ordersDelivered,
            OrderDetailsDelivered = orderDetailsDelivered
        };
        return Ok(model);
    }
}