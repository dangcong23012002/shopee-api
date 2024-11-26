using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Project.Models;

public class AdminController : Controller {
    private readonly DatabaseContext _context;
    private readonly IHomeResponsitory _homeResponsitory;
    private readonly ICategoryResponsitory _categoryResponsitory;
    private readonly IProductResponsitory _productResponsitory;
    private readonly IUserResponsitory _userResponsitory;
    private readonly IAdminResponsitory _adminResponsitory;
    private readonly IHttpContextAccessor _accessor;
    private readonly IOrderResponsitory _orderResponsitory;
    private readonly ICheckoutResponsitory _checkoutResponsitory;
    private readonly IShippingOrderRepository _shippingOrderRepository;
    private readonly ISellerResponsitory _sellerResponsitory;
    public AdminController(
        DatabaseContext context, 
        IAdminResponsitory adminResponsitory, 
        IHomeResponsitory homeResponsitory,
        ICategoryResponsitory categoryResponsitory,
        IProductResponsitory productResponsitory,
        IHttpContextAccessor accessor, 
        IOrderResponsitory orderResponsitory, 
        ICheckoutResponsitory checkoutResponsitory, 
        IUserResponsitory userResponsitory, 
        IShippingOrderRepository shippingOrderRepository,
        ISellerResponsitory sellerResponsitory)
    {
        _context = context;
        _adminResponsitory = adminResponsitory;
        _homeResponsitory = homeResponsitory;
        _categoryResponsitory = categoryResponsitory;
        _productResponsitory = productResponsitory;
        _accessor = accessor;
        _orderResponsitory = orderResponsitory;
        _checkoutResponsitory = checkoutResponsitory;
        _userResponsitory = userResponsitory;
        _shippingOrderRepository = shippingOrderRepository;
        _sellerResponsitory = sellerResponsitory;
    }

    [HttpGet]
    [Route("/admin")]
    public IActionResult Index() {
        IEnumerable<Order> ordersWaitSettlment = _adminResponsitory.getOrdersWaitSettlment().ToList();
        IEnumerable<Order> ordersWaitPickup = _adminResponsitory.getOrderWaitPickup();
        IEnumerable<Order> ordersPicking = _adminResponsitory.getOrdersPicking();
        IEnumerable<Order> ordersDelivering = _adminResponsitory.getOrderDelivering();
        IEnumerable<Order> ordersCompleted = _adminResponsitory.getOrderCompleted();
        IEnumerable<Industry> industries = _categoryResponsitory.getIndustries();
        IEnumerable<CategoryModel> categories = _categoryResponsitory.getAllCategories();
        IEnumerable<UserInfo> userInfos = _userResponsitory.getUsersInfo();
        AdminViewModel model = new AdminViewModel {
            OrdersWaitSettlment = ordersWaitSettlment,
            OrdersWaitPickup = ordersWaitPickup,
            OrdersPicking = ordersPicking,
            OrdersDelivering = ordersDelivering,
            OrdersCompleted = ordersCompleted,
            Industries = industries,
            Categories = categories,
            UserInfos = userInfos,
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/admin/detail-api")]
    public IActionResult Detail(int industryID = 0, int categoryID = 0) {
        IEnumerable<Industry> industry = _categoryResponsitory.getIndustryByID(industryID);
        IEnumerable<CategoryModel> category = _categoryResponsitory.getCategoryByID(categoryID);
        IEnumerable<Industry> industries = _categoryResponsitory.getIndustries();
        AdminViewModel model = new AdminViewModel {
            Industry = industry,
            Category = category,
            Industries = industries
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/admin/add-industry")]
    public IActionResult AddIndustry(string industryName = "", string industryImage = "") {
        Status status;
        if (_categoryResponsitory.insertIndustry(industryName, industryImage)) {
            status = new Status {
                StatusCode = 1,
                Message = "Thêm ngành hàng thành công!"
            };
        } else {
            status = new Status {
                StatusCode = -1,
                Message = "Thêm ngành hàng thất bại!"
            };
        }
        IEnumerable<Industry> industries = _categoryResponsitory.getIndustries();
        AdminViewModel model = new AdminViewModel {
            Status = status,
            Industries = industries
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/admin/update-industry")]
    public IActionResult UpdateIndustry(int industryID = 0, string industryName = "", string industryImage = "") {
        Status status;
        if (_categoryResponsitory.updateIndustry(industryID, industryName, industryImage)) {
            status = new Status {
                StatusCode = 1,
                Message = "Cập nhật ngành hàng thành công!"
            };
        } else {
            status = new Status {
                StatusCode = -1,
                Message = "Cập nhật ngành hàng thất bại!"
            };
        }
        IEnumerable<Industry> industries = _categoryResponsitory.getIndustries();
        AdminViewModel model = new AdminViewModel {
            Status = status,
            Industries = industries
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/admin/delete-industry")]
    public IActionResult DeleteIndustry(int industryID = 0) {
        Status status;
        List<CategoryModel> categoryModels = _categoryResponsitory.getCategoriesByIndustryID(industryID).ToList();
        if (categoryModels.Count() != 0) {
            status = new Status {
                StatusCode = -1,
                Message = "Ngành hàng đang liên quan tới dữ liệu khác, không thể xoá!"
            };
        } else {
            _categoryResponsitory.deleteIndustryByID(industryID);
            status = new Status {
                StatusCode = 1,
                Message = "Xoá ngành hàng thành công!"
            };
        }
        IEnumerable<Industry> industries = _categoryResponsitory.getIndustries();
        AdminViewModel model = new AdminViewModel {
            Status = status,
            Industries = industries
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/admin/add-category")]
    public IActionResult AddCategory(int industryID = 0, string categoryName = "", string categoryImage = "", string categoryDesc = "") {
        Status status;
        if (_categoryResponsitory.inserCategory(industryID, categoryName, categoryImage, categoryDesc)) {
            status = new Status {
                StatusCode = 1,
                Message = "Thêm danh mục thành công!"
            }; 
        } else {
            status = new Status {
                StatusCode = 1,
                Message = "Thêm danh mục thất bại!"
            };
        }
        IEnumerable<CategoryModel> categories = _categoryResponsitory.getAllCategories();
        AdminViewModel model = new AdminViewModel {
            Status = status,
            Categories = categories
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/admin/update-category")]
    public IActionResult UpdateCategory(int categoryID = 0, int industryID = 0, string categoryName = "", string categoryDesc = "", string categoryImage = "") {
        Status status;
        if (_categoryResponsitory.updateCategory(categoryID, industryID, categoryName, categoryDesc, categoryImage)) {
            status = new Status {
                StatusCode = 1,
                Message = "Cập nhật danh mục thành công!"
            };
        } else {
            status = new Status {
                StatusCode = -11,
                Message = "Cập nhật danh mục thất bại!"
            };
        }
        IEnumerable<CategoryModel> categories = _categoryResponsitory.getAllCategories();
        AdminViewModel model = new AdminViewModel {
            Status = status,
            Categories = categories
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("/admin/delete-category")]
    public IActionResult DeleteCategory(int categoryID = 0) {
        Status status;
        List<Product> products = _productResponsitory.getProductsByCategoryID(categoryID).ToList();
        if (products.Count() != 0) {
            status = new Status {
                StatusCode = -1,
                Message = "Thể loại này đang liên quan tới dữ liệu khác, không thể xoá!"
            };
        } else {
            _categoryResponsitory.delelteCategory(categoryID);
            status = new Status {
                StatusCode = 11,
                Message = "Xoá thể loại thành công!"
            };
        }
        IEnumerable<CategoryModel> categories = _categoryResponsitory.getAllCategories();
        AdminViewModel model = new AdminViewModel {
            Status = status,
            Categories = categories
        };
        return Ok(model);
    }

    [HttpGet]
    [Route("/admin/order/{id?}")]
    public IActionResult Order(int id) {
        _accessor?.HttpContext?.Session.SetInt32("CurrentOrderID", id);
        return View();
    }

    [HttpPost]
    [Route("/admin/order")]
    public IActionResult Order() {
        var sessionOrderID = _accessor?.HttpContext?.Session.GetInt32("CurrentOrderID");
        IEnumerable<OrderDetail> orderDetails = _orderResponsitory.getOrderDetailByOrderID(Convert.ToInt32(sessionOrderID));
        List<Order> order = _orderResponsitory.getOrderByOrderID(Convert.ToInt32(sessionOrderID)).ToList();
        List<Address> addresses = _checkoutResponsitory.checkAddressAccount(order[0].FK_iUserID).ToList();
        List<Payment> payments = _checkoutResponsitory.checkPaymentsTypeByUserID(Convert.ToInt32(order[0].FK_iUserID)).ToList();
        AdminViewModel model = new AdminViewModel {
            OrdersWaitSettlment = order,
            OrderDetails = orderDetails,
            Addresses = addresses,
            Payments = payments
        };
        return Ok(model);
    }

    [HttpPut]
    [Route("/admin/confirm-order")]
    public IActionResult ConfirmOrder(int orderID = 0) {
        Status status;
        List<Order> order = _orderResponsitory.getOrderWaitSettlementByOrderID(orderID).ToList();
        if (order.Count() == 0) {
            status = new Status {
                StatusCode = -1,
                Message = "Đơn hàng này không có trạng thái mới đặt!"
            };
        } else {
            if (_orderResponsitory.confirmOrderAboutWaitPickup(orderID, order[0].FK_iUserID)) {
                status = new Status {
                    StatusCode = 1,
                    Message = "Xác nhận đơn hàng thành công!"
                };
            } else {
                status = new Status {
                    StatusCode = 1,
                    Message = "Xác nhận đơn hàng thất bại!"
                };
            }
        }
        return Ok(status);
    }

    [HttpGet]
    [Route("/admin/bill/{orderID?}")]
    public IActionResult Bill(int orderID = 0) {
        // Lấy Cookie tài khoản người dùng trên trình duyệt
        var userID = Request.Cookies["UserID"];
        if (userID != null) {
            _accessor?.HttpContext?.Session.SetInt32("UserID", Convert.ToInt32(userID));
        } else {
            return Redirect("/user/login");
        }
        // Lấy đơn vận theo mã đơn hàng
        List<ShippingOrder> shippingOrder = _shippingOrderRepository.getShippingOrderByOrderID(orderID).ToList();
        _accessor?.HttpContext?.Session.SetInt32("CurrentShippingOrderID", shippingOrder[0].PK_iShippingOrderID);
        return View();
    }

    [HttpGet]
    [Route("/admin/bill-api")]
    public IActionResult BillAPI() {
        var sessionShippingOrderID = _accessor?.HttpContext?.Session.GetInt32("CurrentShippingOrderID");
        IEnumerable<SellerInfo> sellerInfos = _sellerResponsitory.getSellerInfoByShippingOrderID(Convert.ToInt32(sessionShippingOrderID));
        List<ShippingOrder> shippingOrders = _shippingOrderRepository.getShippingOrderByID(Convert.ToInt32(sessionShippingOrderID)).ToList();
        IEnumerable<Order> ordersPicking = _orderResponsitory.getOrderWaitPickingUpByOrderID(shippingOrders[0].FK_iOrderID);
        IEnumerable<OrderDetail> orderDetailsPickingUp = _orderResponsitory.getOrderDetailPickingUpByOrderID(shippingOrders[0].FK_iOrderID);
        List<Address> deliveryAddresses = _checkoutResponsitory.getAddressAccountByOrderID(shippingOrders[0].FK_iOrderID).ToList();
        AdminViewModel model = new AdminViewModel {
            ShippingOrders = shippingOrders,
            SellerInfos = sellerInfos,
            OrdersPicking= ordersPicking,
            OrderDetailsPickingUp = orderDetailsPickingUp,
            Addresses = deliveryAddresses
        };
        return Ok(model); 
    }
}