using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Project.Models;

public class CheckoutController : Controller {

    private readonly IHttpContextAccessor _accessor;
    private readonly ICartReponsitory _cartResponsitory;
    private readonly IProductResponsitory _productResponsitory;
    private readonly ICheckoutResponsitory _checkoutResponsitory;
    private readonly IUserResponsitory _userResponsitory;
    private readonly IOrderResponsitory _orderResponsitory;
    public CheckoutController(IHttpContextAccessor accessor, ICartReponsitory cartResponsitory, IProductResponsitory productResponsitory, ICheckoutResponsitory checkoutResponsitory, IUserResponsitory userResponsitory, IOrderResponsitory orderResponsitory)
    {
        _accessor = accessor;
        _cartResponsitory = cartResponsitory;
        _productResponsitory = productResponsitory;
        _checkoutResponsitory = checkoutResponsitory;
        _userResponsitory = userResponsitory;
        _orderResponsitory = orderResponsitory;
    }

    [HttpGet]
    [Route("/checkout/get-data")]
    public IActionResult Index(int userID = 0) {
        List<UserInfo> userInfos = _userResponsitory.getUserInfoByID(userID).ToList();
        List<Address> addresses = _checkoutResponsitory.checkAddressAccount(userID).ToList();
        List<City> cities = _checkoutResponsitory.getCities().ToList();
        List<District> districts = _checkoutResponsitory.getDistricts().ToList();
        List<AddressChoose> addressChooses = _checkoutResponsitory.getAddressChoose().ToList();
        List<Payment> paymentTypes = _checkoutResponsitory.checkPaymentsTypeByUserID(userID).ToList();
        CheckoutViewModel model = new CheckoutViewModel {
            UserInfos = userInfos,
            Addresses = addresses,
            Cities = cities,
            Districts = districts,
            AddressChooses = addressChooses,
            PaymentTypes = paymentTypes
        };
        return Ok(model); 
    }

    [HttpPost]
    [Route("/checkout/add-address")]
    public IActionResult AddAddress(int userID = 0, string phone = "", string address = "") {
        List<User> user = _userResponsitory.checkUserLogin(userID).ToList();
        Status status;
        if (user.Count() == 0) {
            status = new Status {
                StatusCode = -1,
                Message = "Bạn phải đăng nhập mới được thêm vào giỏ hàng!"
            };
        } else {
            _checkoutResponsitory.insertAddressAccount(userID, phone, address);
            status = new Status
            {
                StatusCode = 1,
                Message = "Thêm địa chỉ thành công"
            };
        }
        List<Address> addresses = _checkoutResponsitory.checkAddressAccount(userID).ToList();
        CheckoutViewModel model = new CheckoutViewModel {
            Status = status,
            Addresses = addresses
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/checkout/address-detail")]
    public IActionResult AddressDetail(int addressID, int userID) {
        var address = _checkoutResponsitory.getAddressesByID(addressID, userID);
        DataViewModel model = new DataViewModel {
            Address = address
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/checkout/address-update")]
    public IActionResult AddressUpdate(int addressID, int userID, string fullname = "", string phone = "", string address = "") {
        _checkoutResponsitory.updateAddressAccountUserByID(userID, fullname);
        _checkoutResponsitory.updateAddressAccountByID(addressID, userID, phone, address);
        List<Address> addresses = _checkoutResponsitory.checkAddressAccount(Convert.ToInt32(userID)).ToList();
        Status status = new Status {
            StatusCode = 1,
            Message = "Cập nhật thành công"
        };
        CheckoutViewModel model = new CheckoutViewModel {
            Addresses = addresses,
            Status = status
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/checkout/add-payment")]
    public IActionResult AddPayment(int paymentID) {
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        _checkoutResponsitory.insertPaymentType(paymentID, Convert.ToInt32(sessionUserID));
        List<Payment> paymentTypes = _checkoutResponsitory.checkPaymentsTypeByUserID(Convert.ToInt32(sessionUserID)).ToList();
        Status status = new Status {
            StatusCode = 1,
            Message = "Thêm thành công!"
        };
        CheckoutViewModel model = new CheckoutViewModel {
            Status = status,
            PaymentTypes = paymentTypes
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/checkout/update-payment")]
    public IActionResult UpdatePayment(int paymentID) {
        var sessionUserID = _accessor?.HttpContext?.Session.GetInt32("UserID");
        _checkoutResponsitory.updatePaymentType(paymentID, Convert.ToInt32(sessionUserID));
        List<Payment> paymentTypes = _checkoutResponsitory.checkPaymentsTypeByUserID(Convert.ToInt32(sessionUserID)).ToList();
        Status status = new Status {
            StatusCode = 1,
            Message = "Cập nhật thành công!"
        };
        CheckoutViewModel model = new CheckoutViewModel {
            Status = status,
            PaymentTypes = paymentTypes
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/checkout/add-to-order")]
    public IActionResult AddToOrder(int userID, int shopID, int productID, int quantity, double unitPrice, double money, double totalPrice, int paymentTypeID, int orderStatusID) {
        List<Order> order = _orderResponsitory.getOrderByID(userID, shopID).ToList();
        // Kiểm tra đơn hàng trong ngày của tài khoản đã đăng ký chưa
        int orderID;
        if (order.Count() != 0) {
            orderID = order[0].PK_iOrderID;
        } else {
            _orderResponsitory.inserOrder(userID, shopID, totalPrice, orderStatusID, paymentTypeID);
            List<Order> newOrder = _orderResponsitory.getOrderByID(userID, shopID).ToList();
            orderID = newOrder[0].PK_iOrderID;
        }
        // Thêm vào chi tiết đơn hàng
        _orderResponsitory.inserOrderDetail(orderID, productID, quantity, unitPrice, money);
        // Xoá sản phẩm trong giỏ hàng
        _cartResponsitory.deleteProductInCart(productID, userID);
        Status status = new Status {
            StatusCode = 1,
            Message = "Đặt hàng thành công!"
        };
        IEnumerable<OrderDetail> orderDetails = _orderResponsitory.getOrderDetailByOrderID(order[0].PK_iOrderID);
        DataViewModel model = new DataViewModel {
            Status = status,
            Orders = order,
            OrderDetails = orderDetails
        };
        return Ok(model);
    }
}