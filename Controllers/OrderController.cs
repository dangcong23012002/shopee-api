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

    [HttpPut]
    [Route("/order/confirm-deliverd")]
    public IActionResult Delivered(int orderID = 0, int userID = 0) {
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
        IEnumerable<Order> ordersDelivered = _orderResponsitory.getOrderByUserIDDeliverd(userID);
        IEnumerable<OrderDetail> orderDetailsDelivered = _orderResponsitory.getProductsOrderByUserIDDelivered(userID);
        OrderViewModel model = new OrderViewModel {
            Status = status,
            OrdersDelivered = ordersDelivered,
            OrderDetailsDelivered = orderDetailsDelivered
        };
        return Ok(model);
    }
}