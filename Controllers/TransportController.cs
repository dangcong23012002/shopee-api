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
        } else {
            status = new Status
            {
                StatusCode = 2,
                Message = "Đăng nhập tài khoản người giao thành công!"
            };
        }
        TransportViewModel model = new TransportViewModel {
            Status = status
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/picker-api/{orderID?}")]
    public IActionResult PickerAPI(int orderID = 0) {
        IEnumerable<ShippingOrder> ordersWaitPickup = _transportRepository.getShippingOrdersWaitPickup();
        IEnumerable<ShippingPicker> ordersPickingUp = _transportRepository.getShippingPickerPickingUp();
        IEnumerable<ShippingPicker> ordersAboutedWarehouse = _shippingOrderRepository.getShippingPickerAboutedWarehouse();
        IEnumerable<SellerInfo> sellerInfos = _transportRepository.getSellerInfoByOrderID(orderID);
        IEnumerable<OrderDetail> orderDetails = _transportRepository.getOrderDetailWaitPickupByOrderID(orderID);
        IEnumerable<Payment> payments = _transportRepository.getPaymentsTypeByOrderID(orderID);
        IEnumerable<ShippingOrder> shippingOrders = _shippingOrderRepository.getShippingOrderByOrderID(orderID);
        IEnumerable<ShippingPicker> shippingPickers = _shippingOrderRepository.getShippingPickerByOrderID(orderID);
        TransportViewModel model = new TransportViewModel {
            OrdersWaitPickup = ordersWaitPickup,
            OrdersPickingUp = ordersPickingUp,
            OrdersAboutedWarehouse = ordersAboutedWarehouse,
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
    public IActionResult PickerAPITakeOrder(int shippingOrderID = 0, int pickerID = 0) {
        Status status;
        List<UserInfo> userInfos = _userResponsitory.checkUserInfoByUserID(pickerID).ToList();
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
        TransportViewModel model = new TransportViewModel {
            OrdersWaitPickup = ordersWaitPickup,
            OrdersPickingUp = ordersPickingUp,
            OrdersAboutedWarehouse = ordersAboutedWarehouse,
            Status = status
        };
        return Ok(model);
    }

    [HttpPut]
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

    [HttpPut]
    [Route("/picker-api/complete")]
    public IActionResult PickerAPIComplete(int shippingPickerID = 0) {
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
        TransportViewModel model = new TransportViewModel {
            OrdersWaitPickup = ordersWaitPickup,
            OrdersPickingUp = ordersPickingUp,
            OrdersAboutedWarehouse = ordersAboutedWarehouse,
            Status = status
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/delivery-api/{orderID?}")]
    public IActionResult DeliveryAPI(int orderID = 0, int deliveryID = 0) {
        IEnumerable<ShippingPicker> ordersWaitDelivery = _shippingOrderRepository.getShippingPickerAboutedWarehouse();
        IEnumerable<ShippingDelivery> ordersDelivering = _shippingOrderRepository.getShippingDeliveryByDeliverID(deliveryID);
        IEnumerable<ShippingDelivery> ordersDelivered = _shippingOrderRepository.getShippingDeliveryCompleteByDeliverID(deliveryID);
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
    public IActionResult DeliveryAPITakeOrder(int shippingOrderID = 0, int deliveryID = 0, int orderID = 0, int orderStatusID = 0, string deliveryImage = "") {
        List<UserInfo> userInfos = _userResponsitory.checkUserInfoByUserID(deliveryID).ToList();
        // Thêm đơn hàng giao và xác nhận đơn hàng về đang giao hàng, xác nhận đơn vận lấy về đang giao hàng
        Status status;
        if (
            _transportRepository.insertShippingDelivery(shippingOrderID, deliveryID, orderStatusID, deliveryImage, userInfos[0].sFullName) &&
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
        IEnumerable<ShippingDelivery> ordersDelivering = _shippingOrderRepository.getShippingDeliveryByDeliverID(deliveryID);
        IEnumerable<ShippingDelivery> ordersDelivered = _shippingOrderRepository.getShippingDeliveryCompleteByDeliverID(deliveryID);
        TransportViewModel model = new TransportViewModel {
            Status = status,
            OrdersWaitDelivery = ordersWaitDelivery,
            OrdersDelivering = ordersDelivering,
            OrdersDelivered = ordersDelivered,
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/delivery-api/taken")]
    public IActionResult DeliveryAPITakenOrder(int shippingDeliveryID = 0, int deliveryID = 0, int shippingOrderID = 0, int orderID = 0) {
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
        IEnumerable<ShippingDelivery> ordersDelivering = _shippingOrderRepository.getShippingDeliveryByDeliverID(deliveryID);
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
    public IActionResult DeliveryAPIComplete(int shippingDeliveryID = 0, int deliveryID = 0, int shippingOrderID = 0, int orderID = 0, string shippingDeliveryImg = "") {
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
        IEnumerable<ShippingDelivery> ordersDelivering = _shippingOrderRepository.getShippingDeliveryByDeliverID(deliveryID);
        IEnumerable<ShippingDelivery> ordersDelivered = _shippingOrderRepository.getShippingDeliveryCompleteByDeliverID(deliveryID);
        TransportViewModel model = new TransportViewModel {
            Status = status,
            OrdersWaitDelivery = ordersWaitDelivery,
            OrdersDelivering = ordersDelivering,
            OrdersDelivered = ordersDelivered
        };
        return Ok(model);
    }
}