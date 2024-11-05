using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project.Models;

public class NewController : Controller
{
    private readonly DatabaseContext _context;
    public NewController(DatabaseContext context)
    {
        _context = context;
    }

    // [Route("/")]
    public IActionResult Index() {
        var categories = _context.Categories.FromSqlRaw("SELECT PK_iCategoryID, sCategoryName, sCategoryImage, sCategoryDescription FROM tbl_Categories");
        return View(categories);
    }

    [Route("/category/create")]
    public IActionResult Create() {
        return View();
    }

    [Route("/category/detail/{id?}")]
    public IActionResult Detail(int id) {
        SqlParameter categoryIDParam = new SqlParameter("@PK_iCategoryID", id);
        var category = _context.Categories.FromSqlRaw("sp_SelectCategoryByID @PK_iCategoryID", categoryIDParam);
        return View(category);
    }

    // [Route("/category/create")]
    [HttpPost]
    public IActionResult Create(Category category) {
        if (!ModelState.IsValid) {
            return View(category);
        }
        SqlParameter categoryNameParam = new SqlParameter("@sCategoryName", category.sCategoryName);
        SqlParameter categoryImgParam = new SqlParameter("@sCategoryImage", category.sCategoryImage);
        SqlParameter categoryDescParam = new SqlParameter("@sCategoryDescription", category.sCategoryDescription);
        _context.Database.ExecuteSqlRaw("EXEC sp_InsertCategory @sCategoryName, @sCategoryImage, @sCategoryDescription", categoryNameParam, categoryImgParam, categoryDescParam);
        TempData["msg"] = "Thêm thể loại thành công";
        return RedirectToAction("Index");
    }

    [Route("/category/delete/{id?}")]
    public IActionResult Delete(int id) {
        SqlParameter categoryIDParam = new SqlParameter("@PK_iCategoryID", id);
        _context.Database.ExecuteSqlRaw("sp_DelelteCategoryByID @PK_iCategoryID", categoryIDParam);
        return RedirectToAction("Index");
    }

    [Route("/category/edit/{id?}")]
    public IActionResult Edit(int id) {
        SqlParameter categoryIDParam = new SqlParameter("@PK_iCategoryID", id);
        var category = _context.Categories.FromSqlRaw("sp_SelectCategoryByID @PK_iCategoryID", categoryIDParam);
        return View(category);
    }

    [HttpPost]
    public IActionResult Update(string categoryName, string categoryImg, string categoryDesc, int categoryId) {
        SqlParameter categoryIDParam = new SqlParameter("@PK_iCategoryID", categoryId);
        SqlParameter categoryNameParam = new SqlParameter("@sCategoryName", categoryName);
        SqlParameter categoryImgParam = new SqlParameter("@sCategoryImage", "dsds");
        SqlParameter categoryDescParam = new SqlParameter("@sCategoryDescription", categoryDesc);
        _context.Database.ExecuteSqlRaw("sp_UpdateCategoryByID @PK_iCategoryID, @sCategoryName, @sCategoryImage, @sCategoryDescription", categoryIDParam, categoryNameParam, categoryImgParam, categoryDescParam);
        string msg = "Sưa thành công";
        return Ok(new {msg});
    }
}