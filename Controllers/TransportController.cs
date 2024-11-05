using Microsoft.AspNetCore.Mvc;
using Project.Models;

public class TransportController : Controller 
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IUserResponsitory _userResponsitory;
    private readonly ITransportRepository _transportRepository;
    private readonly IShippingOrderRepository _shippingOrderRepository;
    private readonly ICheckoutResponsitory _checkoutResponsitory;
    private readonly IOrderResponsitory _orderResponsitory;

    public TransportController(
        IHttpContextAccessor accessor, 
        IUserResponsitory userResponsitory, 
        ITransportRepository transportRepository, 
        IShippingOrderRepository shippingOrderRepository,
        ICheckoutResponsitory checkoutResponsitory,
        IOrderResponsitory orderResponsitory
    )
    {
        _accessor = accessor;
        _userResponsitory = userResponsitory;
        _transportRepository = transportRepository;
        _shippingOrderRepository = shippingOrderRepository;
        _checkoutResponsitory = checkoutResponsitory;
        _orderResponsitory = orderResponsitory;
    }

    [HttpGet]
    [Route("/transport/login")]
    public IActionResult Login() {
        return View();
    }

    [HttpPost]
    [Route("/transport/login")]
    public IActionResult Login(string email = "", string password = "") {
        password = _userResponsitory.encrypt(password);
        Status status;
        List<User> users = _userResponsitory.login(email, password).ToList();
        if (users.Count() == 0)
        {
            status = new Status {
                StatusCode = -1,
                Message = "Tên đăng nhập hoặc mật khẩu không chính xác!"
            };
        }
        else if (users[0].sRoleName != "picker" && users[0].sRoleName != "delivery")
        {
            status = new Status
            {
                StatusCode = 0,
                Message = "Tài khoản không thuộc kênh vận chuyển!"
            };
        } else if (users[0].sRoleName == "picker") {
            status = new Status
            {
                StatusCode = 1,
                Message = "Đăng nhập tài khoản người lấy thành công!"
            };
            string transportUsername = users[0].sUserName;
            string value = users[0].PK_iUserID.ToString();
            // Tạo cookies cho tài khoản người bán
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                IsEssential = true
            };
            Response.Cookies.Append("TransportPickerID", value, options);
            _accessor?.HttpContext?.Session.SetString("TransportPickerUsername", transportUsername);
        } else {
            status = new Status
            {
                StatusCode = 2,
                Message = "Đăng nhập tài khoản người giao thành công!"
            };
            string transportUsername = users[0].sUserName;
            string value = users[0].PK_iUserID.ToString();
            // Tạo cookies cho tài khoản người bán
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1),
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                IsEssential = true
            };
            Response.Cookies.Append("TransportDeliveryID", value, options);
            _accessor?.HttpContext?.Session.SetString("TransportDeliveryUsername", transportUsername);
        }
        TransportViewModel model = new TransportViewModel {
            Status = status
        };
        return Ok(model);
    }

    [Route("/picker")]
    [HttpGet]
    public IActionResult Picker() {
        // Lấy Cookies Người lấy hàng trên trình duyệt
        var pickerID = Request.Cookies["TransportPickerID"];
        if (pickerID != null)
        {
            _accessor?.HttpContext?.Session.SetInt32("TransportPickerID", Convert.ToInt32(pickerID));
        } else {
            return Redirect("/transport/login");
        }
        var sessionPickerID = _accessor?.HttpContext?.Session.GetInt32("TransportPickerID");
        if (sessionPickerID != null)
        {
            List<User> users = _userResponsitory.checkUserLogin(Convert.ToInt32(sessionPickerID)).ToList();
            _accessor?.HttpContext?.Session.SetString("TransportPickerUsername", users[0].sUserName);
            _accessor?.HttpContext?.Session.SetInt32("RoleID", users[0].FK_iRoleID);
        }
        else
        {
            _accessor?.HttpContext?.Session.SetString("TransportPickerUsername", "");
        }
        return View();
    }  

    [HttpGet]
    [Route("/picker-api/{orderID?}")]
    public IActionResult PickerAPI(int orderID = 0) {
        IEnumerable<ShippingOrder> ordersWaitPickup = _transportRepository.getShippingOrdersWaitPickup();
        IEnumerable<ShippingPicker> ordersPickingUp = _transportRepository.getShippingPickerPickingUp();
        IEnumerable<ShippingPicker> ordersAboutedWarehouse = _shippingOrderRepository.getShippingPickerAboutedWarehouse();
        string htmlOrdersWaitPickupItem = "";
        foreach (var item in ordersWaitPickup) {
            htmlOrdersWaitPickupItem += $"  <div class='phone-pickup__work'>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.sFullName}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrdersWaitPickupItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrdersWaitPickupItem += $"          </div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"  </div>";
        }
        string htmlOrderPickingUpItem = "";
        foreach (var item in ordersPickingUp) {
            htmlOrderPickingUpItem += $"  <div class='phone-pickup__work'>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.sFullName}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrderPickingUpItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrderPickingUpItem += $"          </div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"  </div>";
        }

        string htmlOrderAboutedWarehouseItem = "";
        foreach (var item in ordersAboutedWarehouse) {
            htmlOrderAboutedWarehouseItem += $"  <div class='phone-pickup__work'>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.sFullName}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrderAboutedWarehouseItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrderAboutedWarehouseItem += $"          </div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"  </div>";
        }
        IEnumerable<SellerInfo> sellerInfos = _transportRepository.getSellerInfoByOrderID(orderID);
        IEnumerable<OrderDetail> orderDetails = _transportRepository.getOrderDetailWaitPickupByOrderID(orderID);
        IEnumerable<Payment> payments = _transportRepository.getPaymentsTypeByOrderID(orderID);
        IEnumerable<ShippingOrder> shippingOrders = _shippingOrderRepository.getShippingOrderByOrderID(orderID);
        IEnumerable<ShippingPicker> shippingPickers = _shippingOrderRepository.getShippingPickerByOrderID(orderID);
        TransportViewModel model = new TransportViewModel {
            OrdersWaitPickup = ordersWaitPickup,
            OrdersPickingUp = ordersPickingUp,
            OrdersAboutedWarehouse = ordersAboutedWarehouse,
            HtmlOrdersWaitPickupItem = htmlOrdersWaitPickupItem,
            HtmlOrderPickingUpItem = htmlOrderPickingUpItem,
            HtmlOrderAboutedWarehouseItem = htmlOrderAboutedWarehouseItem,
            SellerInfos = sellerInfos,
            OrderDetails = orderDetails,
            Payments = payments,
            ShippingOrders = shippingOrders,
            ShippingPickers = shippingPickers
        };
        return Ok(model);
    }  

    [HttpPost]
    [Route("/picker-api/take")]
    public IActionResult PickerAPITakeOrder(int shippingOrderID = 0) {
        var sessionPickerID = _accessor?.HttpContext?.Session.GetInt32("TransportPickerID");
        Status status;
        List<UserInfo> userInfos = _userResponsitory.checkUserInfoByUserID(Convert.ToInt32(sessionPickerID)).ToList();
        // Thêm đơn hàng lấy
        if (_transportRepository.confirmShippingOrderAboutWaitPickerTake(shippingOrderID) &&
            _transportRepository.insertShippingPicker(shippingOrderID, userInfos[0].sFullName, "no_img.jpg")
            ) {
            status = new Status
            {
                StatusCode = 1,
                Message = "Nhận đơn thành công!"
            };
        } else {
            status = new Status
            {
                StatusCode = 1,
                Message = "Nhận đơn thành công!"
            };
        }
        IEnumerable<ShippingOrder> ordersWaitPickup = _transportRepository.getShippingOrdersWaitPickup();
        IEnumerable<ShippingPicker> ordersPickingUp = _transportRepository.getShippingPickerPickingUp();
        IEnumerable<ShippingPicker> ordersAboutedWarehouse = _shippingOrderRepository.getShippingPickerAboutedWarehouse();
        string htmlOrdersWaitPickupItem = "";
        foreach (var item in ordersWaitPickup) {
            htmlOrdersWaitPickupItem += $"  <div class='phone-pickup__work'>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.sFullName}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrdersWaitPickupItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrdersWaitPickupItem += $"          </div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"  </div>";
        }
        string htmlOrderPickingUpItem = "";
        foreach (var item in ordersPickingUp) {
            htmlOrderPickingUpItem += $"  <div class='phone-pickup__work'>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.sFullName}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrderPickingUpItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrderPickingUpItem += $"          </div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"  </div>";
        }

        string htmlOrderAboutedWarehouseItem = "";
        foreach (var item in ordersAboutedWarehouse) {
            htmlOrderAboutedWarehouseItem += $"  <div class='phone-pickup__work'>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.sFullName}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrderAboutedWarehouseItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrderAboutedWarehouseItem += $"          </div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"  </div>";
        }
        TransportViewModel model = new TransportViewModel {
            OrdersWaitPickup = ordersWaitPickup,
            OrdersPickingUp = ordersPickingUp,
            OrdersAboutedWarehouse = ordersAboutedWarehouse,
            HtmlOrdersWaitPickupItem = htmlOrdersWaitPickupItem,
            HtmlOrderPickingUpItem = htmlOrderPickingUpItem,
            HtmlOrderAboutedWarehouseItem = htmlOrderAboutedWarehouseItem,
            Status = status
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/picker-api/taken")]
    public IActionResult PickerAPITakenOrder(int shippingPickerID = 0, int shippingOrderID = 0, int orderID = 0, string shippingPickerImg = "") {
        System.Console.WriteLine("orderID: " + orderID);
        Status status;
        if (
            _transportRepository.confirmShippingPickerAboutTaken(shippingPickerID) && 
            _transportRepository.confirmShippingOrderAboutDelivered(shippingOrderID) && 
            _transportRepository.updatePickerImage(shippingPickerID, shippingPickerImg) && 
            _transportRepository.confirmShippingPickerAboutingWarehouse(shippingPickerID)
        ) {
            status = new Status {
                StatusCode = 1,
                Message = "Cập nhật trạng thái thành công!"
            };
        }
        else
        {
            status = new Status
            {
                StatusCode = -1,
                Message = "Cập nhật trạng thái thất bại!"
            };
        }

        IEnumerable<SellerInfo> sellerInfos = _transportRepository.getSellerInfoByOrderID(orderID);
        IEnumerable<OrderDetail> orderDetails = _transportRepository.getOrderDetailWaitPickupByOrderID(orderID);
        IEnumerable<OrderDetail> orderDetailsPickingUp = _transportRepository.getOrderDetailPickingUpByOrderID(orderID);
        IEnumerable<Payment> payments = _transportRepository.getPaymentsTypeByOrderID(orderID);
        IEnumerable<ShippingOrder> shippingOrders = _shippingOrderRepository.getShippingOrderByOrderID(orderID);
        IEnumerable<ShippingPicker> shippingPickers = _shippingOrderRepository.getShippingPickerByOrderID(orderID);
        TransportViewModel model = new TransportViewModel {
            Status = status,
            SellerInfos = sellerInfos,
            OrderDetails = orderDetails,
            OrderDetailsPickingUp = orderDetailsPickingUp,
            Payments = payments,
            ShippingOrders = shippingOrders,
            ShippingPickers = shippingPickers
        };
        return Ok(model);
    } 

    [HttpPost]
    [Route("/picker-api/complete")]
    public IActionResult PickerAPIComplete(int shippingPickerID) {
        Status status;
        if (_transportRepository.confirmShippingPickerAboutedWarehouse(shippingPickerID)) {
            status = new Status {
                StatusCode = 1,
                Message = "Đơn hàng đã xong!"
            };
        } else {
            status = new Status {
                StatusCode = -1,
                Message = "Cập nhật trạng thái thất bại!"
            };
        }

        IEnumerable<ShippingOrder> ordersWaitPickup = _transportRepository.getShippingOrdersWaitPickup();
        IEnumerable<ShippingPicker> ordersPickingUp = _transportRepository.getShippingPickerPickingUp();
        IEnumerable<ShippingPicker> ordersAboutedWarehouse = _shippingOrderRepository.getShippingPickerAboutedWarehouse();
        string htmlOrdersWaitPickupItem = "";
        foreach (var item in ordersWaitPickup) {
            htmlOrdersWaitPickupItem += $"  <div class='phone-pickup__work'>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.sFullName}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrdersWaitPickupItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrdersWaitPickupItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrdersWaitPickupItem += $"          </div>";
            htmlOrdersWaitPickupItem += $"      </div>";
            htmlOrdersWaitPickupItem += $"  </div>";
        }
        string htmlOrderPickingUpItem = "";
        foreach (var item in ordersPickingUp) {
            htmlOrderPickingUpItem += $"  <div class='phone-pickup__work'>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.sFullName}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrderPickingUpItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrderPickingUpItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrderPickingUpItem += $"          </div>";
            htmlOrderPickingUpItem += $"      </div>";
            htmlOrderPickingUpItem += $"  </div>";
        }

        string htmlOrderAboutedWarehouseItem = "";
        foreach (var item in ordersAboutedWarehouse) {
            htmlOrderAboutedWarehouseItem += $"  <div class='phone-pickup__work'>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.sFullName}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrderAboutedWarehouseItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrderAboutedWarehouseItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrderAboutedWarehouseItem += $"          </div>";
            htmlOrderAboutedWarehouseItem += $"      </div>";
            htmlOrderAboutedWarehouseItem += $"  </div>";
        }
        TransportViewModel model = new TransportViewModel {
            OrdersWaitPickup = ordersWaitPickup,
            OrdersPickingUp = ordersPickingUp,
            OrdersAboutedWarehouse = ordersAboutedWarehouse,
            HtmlOrdersWaitPickupItem = htmlOrdersWaitPickupItem,
            HtmlOrderPickingUpItem = htmlOrdersWaitPickupItem,
            HtmlOrderAboutedWarehouseItem = htmlOrderAboutedWarehouseItem,
            Status = status
        };
        return Ok(model);
    }

    [Route("/delivery")]
    [HttpGet]
    public IActionResult Delivery() {
        // Lấy cookie người lấy hàng trên trình duyệt
        var deliveryID = Request.Cookies["TransportDeliveryID"];
        if (deliveryID != null) {
            _accessor?.HttpContext?.Session.SetInt32("TransportDeliveryID", Convert.ToInt32(deliveryID));
        } else {
            return Redirect("/transport/login");
        }
        var sessionDeliveryID = _accessor?.HttpContext?.Session.GetInt32("TransportDeliveryID");
        if (sessionDeliveryID != null) {
            List<User> users = _userResponsitory.checkUserLogin(Convert.ToInt32(sessionDeliveryID)).ToList();
            _accessor?.HttpContext?.Session.SetString("TransportDeliveryUsername", users[0].sUserName);
            _accessor?.HttpContext?.Session.SetInt32("RoleID", users[0].FK_iRoleID);
        } else {
            _accessor?.HttpContext?.Session.SetString("TransportDeliveryUsername", "");
        }
        return View();
    }

    [HttpPost]
    [Route("/delivery-api")]
    public IActionResult DeliveryAPI(int orderID = 0) {
        var sessionDeliveryID = _accessor?.HttpContext?.Session.GetInt32("TransportDeliveryID");
        IEnumerable<ShippingPicker> ordersWaitDelivery = _shippingOrderRepository.getShippingPickerAboutedWarehouse();
        IEnumerable<ShippingDelivery> ordersDelivering = _shippingOrderRepository.getShippingDeliveryByDeliverID(Convert.ToInt32(sessionDeliveryID));
        IEnumerable<ShippingDelivery> ordersDelivered = _shippingOrderRepository.getShippingDeliveryCompleteByDeliverID(Convert.ToInt32(sessionDeliveryID));
        string htmlOrdersWaitDeliveryItem = "";
        foreach (var item in ordersWaitDelivery) {
            htmlOrdersWaitDeliveryItem += $"  <div class='phone-pickup__work'>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.sFullName}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrdersWaitDeliveryItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrdersWaitDeliveryItem += $"          </div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"  </div>";
        }

        string htmlOrdersDeliveringItem = "";
        foreach (var item in ordersDelivering) {
            htmlOrdersDeliveringItem += $"  <div class='phone-pickup__work'>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.sBuyerName}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrdersDeliveringItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrdersDeliveringItem += $"          </div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"  </div>";
        }

        string htmlOrdersDeliveredItem = "";
        foreach (var item in ordersDelivered) {
            htmlOrdersDeliveredItem += $"  <div class='phone-pickup__work'>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.sBuyerName}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrdersDeliveredItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrdersDeliveredItem += $"          </div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"  </div>";
        }
        IEnumerable<Address> deliveryAddresses = _checkoutResponsitory.getAddressAccountByOrderID(orderID);
        IEnumerable<OrderDetail> orderDetails = _transportRepository.getOrderDetailShippingDeliveryByOrderID(orderID);
        IEnumerable<Payment> payments = _transportRepository.getPaymentsTypeByOrderID(orderID);
        IEnumerable<ShippingPicker> shippingWaitDelivery = _shippingOrderRepository.getShippingPickersAboutWarehouseByOrderID(orderID);
        IEnumerable<ShippingDelivery> shippingDelivering = _shippingOrderRepository.getShippingDeliveryByOrderID(orderID);
        IEnumerable<ShippingDelivery> shippingDelivered = _shippingOrderRepository.getShippingDeliveredByOrderID(orderID);
        TransportViewModel model = new TransportViewModel {
            OrdersWaitDelivery = ordersWaitDelivery,
            OrdersDelivering = ordersDelivering,
            OrdersDelivered = ordersDelivered,
            HtmlOrdersWaitDeliveryItem = htmlOrdersWaitDeliveryItem,
            HtmlOrdersDeliveringItem = htmlOrdersDeliveringItem,
            HtmlOrdersDeliveredItem = htmlOrdersDeliveredItem,
            DeliveryAddresses = deliveryAddresses,
            OrderDetails = orderDetails,
            Payments = payments,
            ShippingWaitDelivery = shippingWaitDelivery,
            ShippingDelivering = shippingDelivering,
            ShippingDelivered = shippingDelivered
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/delivery-api/take")]
    public IActionResult DeliveryAPITakeOrder(int shippingOrderID = 0, int orderID = 0, int orderStatusID = 0, string deliveryImage = "") {
        var sessionDeliveryID = _accessor?.HttpContext?.Session.GetInt32("TransportDeliveryID");
        List<UserInfo> userInfos = _userResponsitory.checkUserInfoByUserID(Convert.ToInt32(sessionDeliveryID)).ToList();
        // Thêm đơn hàng giao và xác nhận đơn hàng về đang giao hàng, xác nhận đơn vận lấy về đang giao hàng
        Status status;
        if (
            _transportRepository.insertShippingDelivery(shippingOrderID, Convert.ToInt32(sessionDeliveryID), orderStatusID, deliveryImage, userInfos[0].sFullName) &&
            _orderResponsitory.confirmOrderAboutWaitDelivering(orderID) &&
            _transportRepository.confirmShippingPickerAboutDelivering(shippingOrderID)
            ) {
            status = new Status {
                StatusCode = 1,
                Message = "Thêm đơn thành công!"
            };
        } else {
            status = new Status {
                StatusCode = -1,
                Message = "Thêm đơn thất bại!"
            };
        }
        IEnumerable<ShippingPicker> ordersWaitDelivery = _shippingOrderRepository.getShippingPickerAboutedWarehouse();
        IEnumerable<ShippingDelivery> ordersDelivering = _shippingOrderRepository.getShippingDeliveryByDeliverID(Convert.ToInt32(sessionDeliveryID));
        IEnumerable<ShippingDelivery> ordersDelivered = _shippingOrderRepository.getShippingDeliveryCompleteByDeliverID(Convert.ToInt32(sessionDeliveryID));
        string htmlOrdersWaitDeliveryItem = "";
        foreach (var item in ordersWaitDelivery) {
            htmlOrdersWaitDeliveryItem += $"  <div class='phone-pickup__work'>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.sFullName}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrdersWaitDeliveryItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrdersWaitDeliveryItem += $"          </div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"  </div>";
        }

        string htmlOrdersDeliveringItem = "";
        foreach (var item in ordersDelivering) {
            htmlOrdersDeliveringItem += $"  <div class='phone-pickup__work'>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.sBuyerName}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrdersDeliveringItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrdersDeliveringItem += $"          </div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"  </div>";
        }

        string htmlOrdersDeliveredItem = "";
        foreach (var item in ordersDelivered) {
            htmlOrdersDeliveredItem += $"  <div class='phone-pickup__work'>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.sBuyerName}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrdersDeliveredItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrdersDeliveredItem += $"          </div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"  </div>";
        }
        TransportViewModel model = new TransportViewModel {
            Status = status,
            OrdersWaitDelivery = ordersWaitDelivery,
            OrdersDelivering = ordersDelivering,
            OrdersDelivered = ordersDelivered,
            HtmlOrdersWaitDeliveryItem = htmlOrdersWaitDeliveryItem,
            HtmlOrdersDeliveringItem = htmlOrdersDeliveringItem,
            HtmlOrdersDeliveredItem = htmlOrdersDeliveredItem,
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/delivery-api/taken")]
    public IActionResult DeliveryAPITakenOrder(int shippingDeliveryID = 0, int shippingOrderID = 0, int orderID = 0) {
        var sessionDeliveryID = _accessor?.HttpContext?.Session.GetInt32("TransportDeliveryID");
        Status status;
        if (
            _transportRepository.confirmShippingPickerAboutedDeliveryTaken(shippingOrderID) &&
            _transportRepository.confirmShippingDeliveryAboutedDelivering(shippingDeliveryID)
        ) {
            status = new Status {
                StatusCode = 1,
                Message = "Cập nhật trạng thái thành công!"
            };
        } else {
            status = new Status {
                StatusCode = -1,
                Message = "Cập nhật trạng thái thất bại!"
            };
        }
        IEnumerable<ShippingDelivery> ordersDelivering = _shippingOrderRepository.getShippingDeliveryByDeliverID(Convert.ToInt32(sessionDeliveryID));
        IEnumerable<ShippingPicker> shippingWaitDelivery = _shippingOrderRepository.getShippingPickersAboutWarehouseByOrderID(orderID);
        IEnumerable<ShippingDelivery> shippingDeliverring = _shippingOrderRepository.getShippingDeliveryByOrderID(orderID);
        IEnumerable<ShippingDelivery> shippingDelivered = _shippingOrderRepository.getShippingDeliveredByOrderID(orderID);
        IEnumerable<Address> deliveryAddresses = _checkoutResponsitory.getAddressAccountByOrderID(orderID);
        IEnumerable<OrderDetail> orderDetails = _transportRepository.getOrderDetailShippingDeliveryByOrderID(orderID);
        IEnumerable<Payment> payments = _transportRepository.getPaymentsTypeByOrderID(orderID);
        TransportViewModel model = new TransportViewModel {
            Status = status,
            OrdersDelivering = ordersDelivering,
            ShippingWaitDelivery = shippingWaitDelivery,
            ShippingDelivering = shippingDeliverring,
            ShippingDelivered = shippingDelivered,
            DeliveryAddresses = deliveryAddresses,
            OrderDetails = orderDetails,
            Payments = payments
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/delivery-api/complete")]
    public IActionResult DeliveryAPIComplete(int shippingDeliveryID = 0, int shippingOrderID = 0, int orderID = 0, string shippingDeliveryImg = "") {
        var sessionDeliveryID = _accessor?.HttpContext?.Session.GetInt32("TransportDeliveryID");
        Status status;
        if (
            _transportRepository.updateDeliveryImage(shippingDeliveryID, shippingDeliveryImg) &&
            _transportRepository.confirmShippingDeliveryAboutedDeliveredToBuyer(shippingDeliveryID) &&
            _transportRepository.confirmShippingOrderAboutDeliveredBuyer(shippingOrderID) &&
            _orderResponsitory.confirmOrderAboutDelivered(orderID)
            ) {
            status = new Status {
                StatusCode = 1,
                Message = "Đơn hàng đã xong!"
            };
        } else {
            status = new Status {
                StatusCode = -1,
                Message = "Cập nhật trạng thái thất bại!"
            };
        }
        IEnumerable<ShippingPicker> ordersWaitDelivery = _shippingOrderRepository.getShippingPickerAboutedWarehouse();
        IEnumerable<ShippingDelivery> ordersDelivering = _shippingOrderRepository.getShippingDeliveryByDeliverID(Convert.ToInt32(sessionDeliveryID));
        IEnumerable<ShippingDelivery> ordersDelivered = _shippingOrderRepository.getShippingDeliveryCompleteByDeliverID(Convert.ToInt32(sessionDeliveryID));

        string htmlOrdersWaitDeliveryItem = "";
        foreach (var item in ordersWaitDelivery) {
            htmlOrdersWaitDeliveryItem += $"  <div class='phone-pickup__work'>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.sFullName}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrdersWaitDeliveryItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrdersWaitDeliveryItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrdersWaitDeliveryItem += $"          </div>";
            htmlOrdersWaitDeliveryItem += $"      </div>";
            htmlOrdersWaitDeliveryItem += $"  </div>";
        }

        string htmlOrdersDeliveringItem = "";
        foreach (var item in ordersDelivering) {
            htmlOrdersDeliveringItem += $"  <div class='phone-pickup__work'>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.sBuyerName}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrdersDeliveringItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrdersDeliveringItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrdersDeliveringItem += $"          </div>";
            htmlOrdersDeliveringItem += $"      </div>";
            htmlOrdersDeliveringItem += $"  </div>";
        }

        string htmlOrdersDeliveredItem = "";
        foreach (var item in ordersDelivered) {
            htmlOrdersDeliveredItem += $"  <div class='phone-pickup__work'>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Mã đơn hàng</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>ĐH{item.FK_iOrderID}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Khách hàng</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.sBuyerName}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Ngày đặt</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.dDate.ToString("dd/MM/yyyy")}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Tổng tiền</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.fTotalPrice.ToString("#,##0.00")} VNĐ</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Trạng thái</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.sOrderStatusName}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'>Thanh toán</div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>{item.sPaymentName}</div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"      <div class='phone-pickup__work-row'>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-1'></div>";
            htmlOrdersDeliveredItem += $"          <div class='phone-pickup__work-col-2'>";
            htmlOrdersDeliveredItem += $"              <a href='javascript:openOrderDetail({item.FK_iOrderID})' class='phone-pickup__work-link'>Chi tiết đơn</a>";
            htmlOrdersDeliveredItem += $"          </div>";
            htmlOrdersDeliveredItem += $"      </div>";
            htmlOrdersDeliveredItem += $"  </div>";
        }

        TransportViewModel model = new TransportViewModel {
            Status = status,
            OrdersWaitDelivery = ordersWaitDelivery,
            OrdersDelivering = ordersDelivering,
            OrdersDelivered = ordersDelivered,
            HtmlOrdersWaitDeliveryItem = htmlOrdersWaitDeliveryItem,
            HtmlOrdersDeliveringItem = htmlOrdersDeliveringItem,
            HtmlOrdersDeliveredItem = htmlOrdersDeliveredItem
        };
        return Ok(model);
    }
}