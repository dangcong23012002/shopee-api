using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Project.Models.Domain;

public class SellerController : Controller
{
    private readonly IUserResponsitory _userResponsitory;
    private readonly ISellerResponsitory _sellerResponsitory;
    private readonly IShopResponsitory _shopResponsitory;
    private readonly IOrderResponsitory _orderResponsitory;
    private readonly IShippingOrderRepository _shippingOrderRepository;
    private readonly ICheckoutResponsitory _checkoutResponsitory;
    private readonly IHttpContextAccessor _accessor;
    private readonly ICategoryResponsitory _categoryResponsitory;
    private readonly IProductResponsitory _productResponsitory;
    public SellerController(
        IHttpContextAccessor accessor, 
        IUserResponsitory userResponsitory, 
        ISellerResponsitory sellerResponsitory, 
        IShopResponsitory shopResponsitory, 
        IOrderResponsitory orderResponsitory, 
        IShippingOrderRepository shippingOrderRepository, 
        ICheckoutResponsitory checkoutResponsitory,
        ICategoryResponsitory categoryResponsitory,
        IProductResponsitory productResponsitory)
    {
        _accessor = accessor;
        _userResponsitory = userResponsitory;
        _sellerResponsitory = sellerResponsitory;
        _shopResponsitory = shopResponsitory;
        _orderResponsitory = orderResponsitory;
        _shippingOrderRepository = shippingOrderRepository;
        _checkoutResponsitory = checkoutResponsitory;
        _categoryResponsitory = categoryResponsitory;
        _productResponsitory = productResponsitory;
    }

    [HttpGet]
    [Route("/seller")]
    public IActionResult Index(int sellerID = 0) {
        IEnumerable<Seller> sellers = _sellerResponsitory.getSellers();
        List<Store> store = _shopResponsitory.getShopBySellerID(sellerID).ToList();
        List<SellerInfo> sellerInfo = _sellerResponsitory.getSellerInfoBySellerID(sellerID).ToList();
        List<Seller> seller = _sellerResponsitory.getSellerAccountByID(sellerID).ToList();
        DataViewModel model = new DataViewModel {
            Sellers = sellers,
            Store = store,
            SellerInfo = sellerInfo,
            Seller = seller
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/seller")]
    public IActionResult GetData() {
        var sessionSellerID = _accessor?.HttpContext?.Session.GetInt32("SellerID");
        var sessionSellerUsername = _accessor?.HttpContext?.Session.GetString("SellerUsername");
        var sessionShopID = _accessor?.HttpContext?.Session.GetInt32("SellerShopID");
        IEnumerable<SellerInfo> sellerInfos = _sellerResponsitory.getSellerInfoBySellerID(Convert.ToInt32(sessionSellerID));
        IEnumerable<Order> ordersWaitSettlement = _orderResponsitory.getOrderWaitSettlementByShopID(Convert.ToInt32(sessionShopID));
        IEnumerable<Order> ordersWaitPickup = _orderResponsitory.getOrderWaitPickupByShopID(Convert.ToInt32(sessionShopID));
        IEnumerable<ShippingOrder> shippingOrders = _shippingOrderRepository.getShippingOrderByShopID(Convert.ToInt32(sessionShopID));
        IEnumerable<CategoryModel> categories = _categoryResponsitory.getAllCategoriesByShopID(Convert.ToInt32(sessionShopID));
        IEnumerable<Discount> discounts = _productResponsitory.getDiscounts();
        IEnumerable<TransportPrice> transportPrices = _productResponsitory.getTransportPrice();
        IEnumerable<Product> products = _shopResponsitory.getProductsByShopID(Convert.ToInt32(sessionShopID));

        SellerViewModel model = new SellerViewModel {
            SellerID = Convert.ToInt32(sessionSellerID),
            SellerUsername = sessionSellerUsername,
            SellerInfos = sellerInfos,
            OrdersWaitSettlement = ordersWaitSettlement,
            OrdersWaitPickup = ordersWaitPickup,
            OrdersProcessed = shippingOrders,
            ShippingOrders = shippingOrders,
            Categories = categories,
            Discounts = discounts,
            TransportPrices = transportPrices,
            Products = products,
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/seller/product-detail/{productID}")]
    public IActionResult ProductDetailAPI(int productID = 0) {
        List<Product> products = _productResponsitory.getProductByID(productID).ToList();
        IEnumerable<CategoryModel> categories = _categoryResponsitory.getAllCategoriesByShopID(products[0].FK_iStoreID);
        IEnumerable<Discount> discounts = _productResponsitory.getDiscounts();
        IEnumerable<TransportPrice> transportPrices = _productResponsitory.getTransportPrice();
        SellerViewModel model = new SellerViewModel
        {
            Categories = categories,
            Discounts = discounts,
            TransportPrices = transportPrices,
            Products = products
        };
        return Ok(model);
    }

    [HttpPut]
    [Route("/seller/update-product")]
    public IActionResult UpdateProduct(int productID = 0, int shopID = 0, int categoryID = 0, int discountID = 0, int transportID = 0, string productName = "", int quantity = 0, string productDesc = "", string imageUrl = "", double price = 0) {
        _productResponsitory.updateProduct(productID, shopID, categoryID, discountID, transportID, productName, quantity, productDesc, imageUrl, price);
        Status status = new Status {
            StatusCode = 1,
            Message = "Cập nhật sản phẩm thành công"
        };
        IEnumerable<Product> products = _shopResponsitory.getProductsByShopID(shopID);
        SellerViewModel model = new SellerViewModel {
            Status = status,
            Products = products
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/seller/add-product")]
    public IActionResult AddProduct(int categoryID = 0, int discountID = 0, int transportID = 0, string productName = "", int quantity = 0, string productDesc = "", string imageUrl = "", double price = 0) {
        var sessionShopID = _accessor?.HttpContext?.Session.GetInt32("SellerShopID");
        _productResponsitory.insertProduct(Convert.ToInt32(sessionShopID), categoryID, discountID, transportID, productName, quantity, productDesc, imageUrl, price);
        Status status = new Status {
            StatusCode = 1,
            Message = "Thêm sản phẩm thành công!"
        };
        List<Product> product = _productResponsitory.searchProductByKeyword(productName).ToList();
        IEnumerable<Product> products = _shopResponsitory.getProductsByShopID(Convert.ToInt32(sessionShopID));
        string htmlProductItem = "";
        foreach (var item in products) {
            htmlProductItem += $"    <div class='admin__product-item'>";
            htmlProductItem += $"        <div class='admin__product-item-input'>";
            htmlProductItem += $"            <input type='checkbox' class='admin__product-item-input-checkbox'>";
            htmlProductItem += $"        </div>";
            htmlProductItem += $"        <div class='admin__product-item-info'>";
            htmlProductItem += $"           <div class='admin__product-item-img' style='background-image: url(/img/{item.sImageUrl});'></div>";
            htmlProductItem += $"           <div class='admin__product-item-desc'>";
            htmlProductItem += $"               <div class='admin__product-item-name'>{item.sProductName}";
            htmlProductItem += $"                   <div class='cart__body-product-name-progress'>";
            htmlProductItem += $"                       <div class='cart__body-product-name-progress-line'></div>";
            htmlProductItem += $"                       <div class='cart__body-product-name-progress-line'></div>";
            htmlProductItem += $"                   </div>";
            htmlProductItem += $"               </div>";
            htmlProductItem += $"               <img src='/img/voucher.png' class='admin__product-item-voucher' alt=''>";
            htmlProductItem += $"           </div>";
            htmlProductItem += $"       </div>";
            htmlProductItem += $"       <div class='admin__product-item-type'>{item.sCategoryName}</div>";
            htmlProductItem += $"       <div class='admin__product-item-cre-time'>{item.dCreateTime.ToString("dd/MM/yyyy")}</div>";
            htmlProductItem += $"       <div class='admin__product-item-update-time'>{item.dUpdateTime.ToString("dd/MM/yyyy")}</div>";
            htmlProductItem += $"       <div class='admin__product-item-qnt'>{item.iQuantity}</div>";
            htmlProductItem += $"       <div class='admin__product-item-operation'>";
            htmlProductItem += $"           <div class='admin-tool__more'>";
            htmlProductItem += $"               <i class='uil uil-ellipsis-v admin-tool__more-icon'></i>";
            htmlProductItem += $"               <div class='admin-tool__more-container'>";
            htmlProductItem += $"                   <div class='admin-tool__more-item' onclick='openUpdateProduct({item.PK_iProductID})'>";
            htmlProductItem += $"                       <i class='uil uil-pen admin-tool__more-item-icon'></i>";
            htmlProductItem += $"                       <span>Chỉnh sửa</span>";
            htmlProductItem += $"                   </div>";
            htmlProductItem += $"                   <div class='admin-tool__more-item' onclick='openDeleteProduct({item.PK_iProductID})'>";
            htmlProductItem += $"                       <i class='uil uil-trash admin-tool__more-item-icon'></i>";
            htmlProductItem += $"                       <span>Xoá</span>";
            htmlProductItem += $"                   </div>";
            htmlProductItem += $"               </div>";
            htmlProductItem += $"           </div>";
            htmlProductItem += $"       </div>";
            htmlProductItem += $"   </div>";
        }
        SellerViewModel model = new SellerViewModel {
            Status = status,
            Products = products,
            HtmlProductItem = htmlProductItem,
            NewCreatedProductID = product[0].PK_iProductID
        };
        return Ok(model);
    }

    [HttpDelete]
    [Route("/seller/delete-product")]
    public IActionResult DeleteProduct(int productID = 0) {
        Status status;
        List<Product> checkProductInCart = _productResponsitory.checkProductInCart(productID).ToList();
        List<Product> checkProductInOrder = _productResponsitory.checkProductInOrder(productID).ToList();
        if (checkProductInCart.Count() != 0 || checkProductInOrder.Count() != 0) {
            status = new Status {
                StatusCode = -1,
                Message = "Sản phẩm đang liên quan tới dữ liệu khác, không thể xoá!"
            };
        } else {
            _productResponsitory.deleteProductByID(productID);
            status = new Status {
                StatusCode = 1,
                Message = "Xoá sản phẩm thành công"
            };
        }
        var sessionShopID = _accessor?.HttpContext?.Session.GetInt32("SellerShopID");
        IEnumerable<Product> products = _shopResponsitory.getProductsByShopID(Convert.ToInt32(sessionShopID));
        SellerViewModel model = new SellerViewModel {
            Status = status,
            Products = products
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/seller/login")]
    public IActionResult Login(string phone = "", string password = "") {
        Status status;
        string phoneRegex = @"\+?(09|08|07|06|05|03)\d{8}";
        if (phone == null && password == null) {
            status = new Status {
                StatusCode = -1,
                Message = "Số điện thoại, mật khẩu không được trống!"
            };
        } else if (phone == null) {
            status = new Status {
                StatusCode = -1,
                Message = "Số điện thoại không được trống!"
            };
        } else if (password == null) {
            status = new Status {
                StatusCode = -1,
                Message = "Mật khẩu không được trống!"
            };
        } else if (!Regex.IsMatch(phone, phoneRegex)) {
            status = new Status {
                StatusCode = -1,
                Message = "Số điện thoại phải 10 số, bắt đầu bằng số 0!"
            };
        } else {
            List<Seller> sellerLogin = _sellerResponsitory.loginAccount(phone, password).ToList();
            List<SellerInfo> sellerInfos = _sellerResponsitory.getSellerInfoByPhone(phone).ToList();
            if (sellerLogin.Count() == 0) {
                status = new Status {
                    StatusCode = -1,
                    Message = "Số điện thoại hoặc mật khẩu không chính xác!"
                };
            } else if (sellerInfos.Count() == 0) {
                status = new Status {
                    StatusCode = -2,
                    Message = "Tài khoản người bán chưa đầy đủ thông tin!"
                };
            } else {
                status = new Status {
                    StatusCode = 1,
                    Message = "Đăng nhập thành công!"
                };
            }
        }
        IEnumerable<SellerInfo> sellerInfo = _sellerResponsitory.getSellerInfoByPhoneAndPassword(phone, password);
        SellerViewModel model = new SellerViewModel {
            Status = status,
            SellerInfo = sellerInfo
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/seller/register")]
    public IActionResult Register(string phone = "", string username = "", string password = "") {
        // phone = "0" + phone;
        Status status;
        List<Seller> seller = _sellerResponsitory.getPasswordSellerAccountByPhone(phone).ToList();
        if (seller.Count() != 0) {
            status = new Status {
                StatusCode = 1,
                Message = "Số điện thoại này đã được đăng ký!"
            };
        } else if (_sellerResponsitory.registerAccountSeller(phone, username, _userResponsitory.encrypt(password))) {
            status = new Status {
                StatusCode = 1,
                Message = "Đăng ký tài khoản người bán thành công!"
            };
        } else {
            status = new Status {
                StatusCode = -1,
                Message = "Đăng ký tài khoản người bán thất bại!"
            };
        }
        SellerViewModel model = new SellerViewModel {
            Status = status
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/seller/portal")]
    public IActionResult Portal() {
        // Lấy Cookie trên trình duyệt
        var sellerID = Request.Cookies["SellerID"];
        if (sellerID != null) {
            _accessor?.HttpContext?.Session.SetInt32("SellerID", Convert.ToInt32(sellerID));
        } else {
            return Redirect("/seller/login");
        }
        var sessionSellerID = _accessor?.HttpContext?.Session.GetInt32("SellerID");
        List<Seller> seller = _sellerResponsitory.getSellerAccountByID(Convert.ToInt32(sessionSellerID)).ToList();
        _accessor?.HttpContext?.Session.SetString("SellerUsername", seller[0].sSellerUsername);
        return View();
    }

    [HttpPost]
    [Route("/seller/portal-api")]
    public IActionResult PortalAPI() {
        var sessionSellerID = _accessor?.HttpContext?.Session.GetInt32("SellerID");
        var sessionSellerUsername = _accessor?.HttpContext?.Session.GetString("SellerUsername");
        SellerViewModel model = new SellerViewModel {
            SellerID = Convert.ToInt32(sessionSellerID),
            SellerUsername = sessionSellerUsername
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/seller/forgot")]
    public IActionResult Forgot(string phone = "") {
        List<Seller> seller = _sellerResponsitory.getPasswordSellerAccountByPhone(phone).ToList();
        Status status;
        if (seller.Count() == 0) {
            status = new Status {
                StatusCode = -1,
                Message = "Không tồn tại số điện thoại!"
            };
        } else {
            string sellerPassword = _userResponsitory.decrypt(seller[0].sSellerPassword);
            status = new Status {
                StatusCode = 1,
                Message = $"Mật khẩu tài khoản của bạn: {sellerPassword}"
            };
        }
        SellerViewModel model = new SellerViewModel {
            Status = status
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/seller/change")]
    public IActionResult Change(int sellerID, string oldPassword = "", string newPassword = "") {
        oldPassword = _userResponsitory.encrypt(oldPassword);
        newPassword = _userResponsitory.encrypt(newPassword);
        List<Seller> sellerLogin = _sellerResponsitory.checkSellerAccountByIDAndPass(sellerID, oldPassword).ToList();
        Status status;
        if (sellerLogin.Count() == 0)
        {
            status = new Status
            {
                StatusCode = -1,
                Message = "Mật khẩu cũ không chính xác!"
            };
        }
        else
        {
            _sellerResponsitory.changePasswordSellerAccount(sellerID, newPassword);
            status = new Status
            {
                StatusCode = 1,
                Message = "Đổi mật khẩu thành công!"
            };
        }
        SellerViewModel model = new SellerViewModel {
            Status = status
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/seller/logout")]
    public IActionResult Logout() {
        CookieOptions options = new CookieOptions {
            Expires = DateTime.Now.AddDays(-1)
        };
        Response.Cookies.Append("SellerID", "0", options);
        _accessor?.HttpContext?.Session.SetInt32("SellerID", 0);
        Status status = new Status {
            StatusCode = 1,
            Message = "Đăng xuất thành công!"
        };
        return Ok(status);
    }

    [HttpPost]
    [Route("/seller/confirm-shipping-order")]
    public IActionResult ShippingOrder(int orderID = 0, int userID = 0) {
        Status status;
        // Cập nhật đơn hàng về trạng thái đang vận chuyển
        if (_shippingOrderRepository.insertShippingOrder(1, orderID) && _orderResponsitory.confirmOrderAboutTransiting(orderID, userID)) {
            status = new Status
            {
                StatusCode = 1,
                Message = "Phiếu đã được tạo thành công!"
            };
        } else {
            status = new Status
            {
                StatusCode = 1,
                Message = "Phiếu đã được tạo thành công!"
            };
        }
        return Ok(status);
    }

    [HttpGet]
    [Route("/seller/delivery-note/{orderID?}")]
    public IActionResult DeliveryNote(int orderID = 0) {
        // Lấy Cookie trên trình duyệt
        var sellerID = Request.Cookies["SellerID"];
        if (sellerID != null) {
            _accessor?.HttpContext?.Session.SetInt32("SellerID", Convert.ToInt32(sellerID));
        } else {
            return Redirect("/seller/login");
        }
        // Lấy đơn hàng giao theo mã đơn hàng
        List<ShippingOrder> shippingOrder = _shippingOrderRepository.getShippingOrderByOrderID(orderID).ToList();
        _accessor?.HttpContext?.Session.SetInt32("CurrentShippingOrderID", shippingOrder[0].PK_iShippingOrderID);
        return View();
    }

    [HttpGet]
    [Route("/seller/delivery-api")]
    public IActionResult DeliveryNoteAPI(int sellerID = 0, int shippingOrderID = 0) {
        IEnumerable<SellerInfo> sellerInfos = _sellerResponsitory.getSellerInfoBySellerID(sellerID);
        List<ShippingOrder> shippingOrders = _shippingOrderRepository.getShippingOrderByID(shippingOrderID).ToList();
        IEnumerable<Order> ordersWaitDelivery = _orderResponsitory.getOrderWaitDeliveryByOrderID(shippingOrders[0].FK_iOrderID);
        IEnumerable<OrderDetail> orderDetailsWaitDelivery = _orderResponsitory.getOrderDetailWaitDeliveyByOrderID(shippingOrders[0].FK_iOrderID);
        IEnumerable<Address> deliveryAddresses = _checkoutResponsitory.getAddressAccountByOrderID(shippingOrders[0].FK_iOrderID);
        SellerViewModel model = new SellerViewModel {
            ShippingOrders = shippingOrders,
            SellerInfos = sellerInfos,
            OrdersWaitDelivery = ordersWaitDelivery,
            OrderDetailsWaitDelivery = orderDetailsWaitDelivery,
            DeliveryAddresses = deliveryAddresses
        };
        return Ok(model);
    }
}