using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using System.Diagnostics;
using RouteAtrribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly IHomeResponsitory _homeResponsitory;
        private readonly ICartReponsitory _cartResponsitory;
        private readonly IUserResponsitory _userResponsitory;
        private readonly ICategoryResponsitory _categoryResponsitory;

        public HomeController(
            ILogger<HomeController> logger, 
            DatabaseContext context, 
            IHttpContextAccessor accessor, 
            IHomeResponsitory homeResponsitory, 
            ICartReponsitory cartReponsitory, 
            IUserResponsitory userResponsitory,
            ICategoryResponsitory categoryResponsitory
        )
        {
            _logger = logger;
            _context = context;
            _accessor = accessor;
            _homeResponsitory = homeResponsitory;
            _cartResponsitory = cartReponsitory;
            _userResponsitory = userResponsitory;
            _categoryResponsitory = categoryResponsitory;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index(int currentPage = 1, int userID = 0) {
            List<User> users;
            int roleID = 0;
            string username = "";
            if (userID != 0) {
                users = _userResponsitory.checkUserLogin(userID).ToList();
                roleID = users[0].FK_iRoleID;
                username = users[0].sUserName;
            } 
            IEnumerable<Product> products = _homeResponsitory.getProducts().ToList();
            int totalRecord = products.Count();
            int pageSize = 12;
            int totalPage = (int) Math.Ceiling(totalRecord / (double) pageSize);
            products = products.Skip((currentPage - 1) * pageSize).Take(pageSize);
            IEnumerable<Store> stores = _homeResponsitory.getStores();
            IEnumerable<ParentCategory> parentCategories = _homeResponsitory.getParentCategories();
            IEnumerable<Category> categories = _homeResponsitory.getCategories().ToList();
            IEnumerable<Favorite> favorites = _homeResponsitory.getFavorites(userID);
            IEnumerable<CartDetail> cartDetails = _cartResponsitory.getCartInfo(userID).ToList();
            IEnumerable<CartDetail> carts = _cartResponsitory.getCartInfo(userID);
            int cartCount = carts.Count();
            ShopeeViewModel model = new ShopeeViewModel {
                Stores = stores,
                Products = products,
                ParentCategories = parentCategories,
                Categories = categories,
                Favorites = favorites,
                CartDetails = cartDetails,
                TotalPage = totalPage,
                PageSize = pageSize,
                CurrentPage = currentPage,
                RoleID = roleID,
                UserID = userID,
                Username = username,
                CartCount = cartCount
            };
            return Ok(model);
        }

        [HttpGet]
        [Route("/home/search/{keyword?}")]
        public IActionResult Search(string keyword = "") {
            IEnumerable<ParentCategory> parentCategories = _categoryResponsitory.searchParentCategoriesByKeyword(keyword);
            IEnumerable<Category> categories = _categoryResponsitory.searchCategoriesByKeyword(keyword).ToList();
            ShopeeViewModel model = new ShopeeViewModel {
                ParentCategories = parentCategories,
                Categories = categories
            };
            return Ok(model);
        }

        [HttpGet]
        [Route("/home/suggest")]
        public IActionResult Suggest(int currentPage = 1) {
            IEnumerable<Product> products = _homeResponsitory.getProducts().ToList();
            int totalRecord = products.Count();
            int pageSize = 12;
            int totalPage = (int) Math.Ceiling(totalRecord / (double) pageSize);
            products = products.Skip((currentPage - 1) * pageSize).Take(pageSize);
            ShopeeViewModel model = new ShopeeViewModel {
                Products = products,
                TotalPage = totalPage,
                PageSize = pageSize,
                CurrentPage = currentPage
            };
            return Ok(model);
        }
    }
}