using Microsoft.AspNetCore.Mvc;

[Route("/payment")]
public class PaymentController : Controller
{
    private readonly IOrderResponsitory _orderResponsitory;
    private readonly IHttpContextAccessor _accessor;
    public PaymentController(IOrderResponsitory orderResponsitory, IHttpContextAccessor accessor)
    {
        _orderResponsitory = orderResponsitory;
        _accessor = accessor;
    }

    [HttpGet]
    [Route("momo")]
    public IActionResult Momo() {
        // Lấy Cookies trên trình duyệt
        var userID = Request.Cookies["UserID"];
        if (userID != null)
        {
            _accessor?.HttpContext?.Session.SetInt32("UserID", Convert.ToInt32(userID));
        }
        return View();
    }

    [HttpPost]
    [Route("momo")]
    public IActionResult GetDataMomo() {
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        var sessionShopID = _accessor?.HttpContext?.Session.GetInt32("ShopID");
        List<Order> orders = _orderResponsitory.getOrderByID(Convert.ToInt32(sessionUserID), Convert.ToInt32(sessionShopID)).ToList();
        Order order = new Order {
            PK_iOrderID = orders[0].PK_iOrderID,
            fTotalPrice = orders[0].fTotalPrice
        };
        CheckoutViewModel model = new CheckoutViewModel {
            Order = order
        };
        return Ok(model);
    }
}