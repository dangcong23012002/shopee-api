
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project.Models;

public class ShopResponsitory : IShopResponsitory
{
    private readonly DatabaseContext _context;
    public ShopResponsitory(DatabaseContext context)
    {
        _context = context;
    }

    public IEnumerable<Product> getTop10SuggestProductsShop(int shopID)
    {
        SqlParameter shopIDPram = new SqlParameter("@PK_iShopID", shopID);
        return _context.Products.FromSqlRaw("EXEC sp_GetTop10SuggestProductsByShopID @PK_iShopID", shopIDPram);
    }

    public IEnumerable<Category> getCategoriesByShopID(int shopID)
    {
        SqlParameter shopIDParam = new SqlParameter("@PK_iShopID", shopID);
        return _context.Categories.FromSqlRaw("EXEC sp_GetCategoriesByShopID @PK_iShopID", shopIDParam);
    }

    public IEnumerable<Product> getProductsByShopID(int shopID)
    {
        SqlParameter shopIDParam = new SqlParameter("@PK_iShopID", shopID);
        return _context.Products.FromSqlRaw("EXEC sp_GetProductsByShopID @PK_iShopID", shopIDParam);
    }

    public IEnumerable<Store> getShopByID(int shopID)
    {
        SqlParameter shopIDParam = new SqlParameter("@PK_iShopID", shopID);
        return _context.Stores.FromSqlRaw("EXEC sp_GetShopByID @PK_iShopID", shopIDParam);
    }

    public IEnumerable<Product> getTop10GoodPriceProductsShop(int shopID)
    {
        SqlParameter shopIDParam = new SqlParameter("@PK_iShopID", shopID);
        return _context.Products.FromSqlRaw("EXEC sp_GetTop10GoodPriceProductsShop @PK_iShopID", shopIDParam);
    }

    public IEnumerable<Product> getTop10SellingProductsShop(int shopID)
    {
        SqlParameter shopIDParam = new SqlParameter("@PK_iShopID", shopID);
        return _context.Products.FromSqlRaw("EXEC sp_GetTop10SellingProductsShop @PK_iShopID", shopIDParam);
    }

    public IEnumerable<Product> getTop3SellingProductsShop(int shopID)
    {
        SqlParameter shopIDParam = new SqlParameter("@PK_iShopID", shopID);
        return _context.Products.FromSqlRaw("EXEC sp_GetTop3SellingProductsShop @PK_iShopID", shopIDParam);
    }

    public IEnumerable<SliderShop> getSlidersShopByShopID(int shopID)
    {
        SqlParameter shopIDParam = new SqlParameter("@FK_iShopID", shopID);
        return _context.SliderShops.FromSqlRaw("EXEC sp_GetBannersShopByShopID @FK_iShopID", shopIDParam);
    }

    public IEnumerable<Store> getShopByProductID(int productID)
    {
        SqlParameter productIDParam = new SqlParameter("@PK_iProductID", productID);
        return _context.Stores.FromSqlRaw("sp_GetShopByProductID @PK_iProductID", productIDParam);
    }

    public IEnumerable<Store> getShopByParentCategoryID(int parentCategoryID)
    {
        SqlParameter parentCategoryIDParam = new SqlParameter("@FK_iParentCategoryID", parentCategoryID);
        return _context.Stores.FromSqlRaw("EXEC sp_GetShopByParentCategoryID @FK_iParentCategoryID", parentCategoryIDParam);
    }

    public IEnumerable<Store> getShopByUsername(string shopUsername)
    {
        SqlParameter shopUsernameParam = new SqlParameter("@sShopUsername", shopUsername);
        return _context.Stores.FromSqlRaw("EXEC sp_GetShopByUsername @sShopUsername", shopUsernameParam);
    }

    public IEnumerable<Store> getShopBySellerID(int sellerID)
    {
        SqlParameter sellerIDParam = new SqlParameter("@FK_iSellerID", sellerID);
        return _context.Stores.FromSqlRaw("EXEC sp_GetShopBySellerID @FK_iSellerID", sellerIDParam);
    }
}