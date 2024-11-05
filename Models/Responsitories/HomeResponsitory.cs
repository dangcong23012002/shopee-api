
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project.Models;

public class HomeResponsitory : IHomeResponsitory
{
    private readonly DatabaseContext _context;
    public HomeResponsitory(DatabaseContext context)
    {
        _context = context;
    }
    public IEnumerable<Product> getProducts() {
        return _context.Products.FromSqlRaw("EXEC sp_SelectProducts").ToList();
    }
    public IEnumerable<Category> getCategories() { 
        return _context.Categories.FromSqlRaw("EXEC sp_SelectCategories").ToList();
    }

    public IEnumerable<Product> displayProductsPagination(int pageSize, int pageNumber)
    {
        SqlParameter pageSizeParam = new SqlParameter("@PageSize", pageSize);
        SqlParameter pageNumberParam = new SqlParameter("@PageNumber", pageNumber);
        return _context.Products.FromSqlRaw("EXEC sp_PaginationProducts @PageSize, @PageNumber", pageSizeParam, pageNumberParam);
    }

    public IEnumerable<Store> getStores()
    {
        return _context.Stores.FromSqlRaw("EXEC sp_GetStores");
    }

    public IEnumerable<Favorite> getFavorites(int userID)
    {
        SqlParameter userIDParam = new SqlParameter("@FK_iUserID", userID);
        return _context.Favorites.FromSqlRaw("sp_SelectProductFavorites @FK_iUserID", userIDParam);
    }

    public IEnumerable<Category> getCategoriesByParentCategoryID(int parentCategoryID)
    {
        SqlParameter parentCategoryIDParam = new SqlParameter("@FK_iParentCategoryID", parentCategoryID);
        return _context.Categories.FromSqlRaw("EXEC sp_SelectCategoriesByParentCategoryID @FK_iParentCategoryID", parentCategoryIDParam);
    }

    public IEnumerable<ParentCategory> getParentCategories()
    {
        return _context.ParentCategories.FromSqlRaw("EXEC sp_SelectParentCategories");
    }
}