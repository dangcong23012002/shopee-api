using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project.Models;

public class CategoryResponsitory : ICategoryResponsitory
{
    private readonly DatabaseContext _context;
    public CategoryResponsitory(DatabaseContext context)
    {
        _context = context;
    }

    public bool delelteCategory(int categoryID)
    {
        SqlParameter categoryIDParam = new SqlParameter("@PK_iCategoryID", categoryID);
        _context.Database.ExecuteSqlRaw("EXEC sp_DelelteCategoryByID @PK_iCategoryID", categoryIDParam);
        return true;
    }

    public bool deleteIndustryByID(int industryID)
    {
        SqlParameter industryIDParam = new SqlParameter("@PK_iIndustryID", industryID);
        _context.Database.ExecuteSqlRaw("EXEC sp_DeleteIndustryByID @PK_iIndustryID", industryIDParam);
        return true;
    }

    public IEnumerable<CategoryModel> getAllCategories()
    {
        return _context.CategoryModels.FromSqlRaw("EXEC sp_GetAllCategories");
    }

    public IEnumerable<CategoryModel> getAllCategoriesByShopID(int shopID)
    {
        SqlParameter shopIDParam = new SqlParameter("@FK_iShopID", shopID);
        return _context.CategoryModels.FromSqlRaw("EXEC sp_GetAllCategoriesByShopID @FK_iShopID", shopIDParam);
    }

    public IEnumerable<Category> getCategories()
    {
        return _context.Categories.FromSqlRaw("EXEC sp_SelectCategories");
    }

    public IEnumerable<Category> searchCategoriesByKeyword(string keyword) {
        SqlParameter keywordParam = new SqlParameter("@sKeyword", keyword);
        return _context.Categories.FromSqlRaw("EXEC sp_SearchCategoryByKeyword @sKeyword", keywordParam);
    }

    public IEnumerable<CategoryModel> getCategoriesByIndustryID(int industryID)
    {
        SqlParameter industryIDParam = new SqlParameter("@FK_iIndustryID", industryID);
        return _context.CategoryModels.FromSqlRaw("EXEC sp_GetCategoriesByIndustryID @FK_iIndustryID", industryIDParam);
    }

    public IEnumerable<CategoryModel> getCategoryByID(int categoryID)
    {
        SqlParameter categoryIDParam = new SqlParameter("@PK_iCategoryID", categoryID);
        return _context.CategoryModels.FromSqlRaw("EXEC sp_GetCategoryByID @PK_iCategoryID", categoryIDParam);
    }

    public IEnumerable<Industry> getIndustries()
    {
        return _context.Industries.FromSqlRaw("EXEC sp_GetIndustries");
    }

    public IEnumerable<Industry> getIndustryByID(int industryID)
    {
        SqlParameter industryIDParam = new SqlParameter("@PK_iIndustryID", industryID);
        return _context.Industries.FromSqlRaw("EXEC sp_GetIndustryByID @PK_iIndustryID", industryIDParam);
    }

    public bool inserCategory(int industryID, string categoryName, string categoryImage, string categoryDesc)
    {
        SqlParameter industryIDParam = new SqlParameter("@FK_iParentCategoryID", industryID);
        SqlParameter categoryNameParam = new SqlParameter("@sCategoryName", categoryName);
        SqlParameter categoryImageParam = new SqlParameter("@sCategoryImage", categoryImage);
        SqlParameter categoryDescParam = new SqlParameter("@sCategoryDescription", categoryDesc);
        SqlParameter isVisibleParam = new SqlParameter("@iIsVisible", 1);
        SqlParameter createTimeParam = new SqlParameter("@dCreateTime", DateTime.Now);
        SqlParameter updateTimeParam = new SqlParameter("@dUpdateTime", DateTime.Now);
        _context.Database.ExecuteSqlRaw("EXEC sp_InsertCategory @FK_iParentCategoryID, @sCategoryName, @sCategoryImage, @sCategoryDescription, @iIsVisible, @dCreateTime, @dUpdateTime", 
            industryIDParam,
            categoryNameParam, 
            categoryImageParam,
            categoryDescParam,
            isVisibleParam,
            createTimeParam,
            updateTimeParam
        );
        return true;
    }

    public bool insertIndustry(string industryName, string industryImage)
    {
        SqlParameter industryNameParam = new SqlParameter("@sIndustryName", industryName);
        SqlParameter industryImageParam = new SqlParameter("@sIndustryImage", industryImage);
        SqlParameter createTimeParam = new SqlParameter("@dCreateTime", DateTime.Now);
        SqlParameter updateTimeParam = new SqlParameter("@dUpdateTime", DateTime.Now);
        _context.Database.ExecuteSqlRaw("EXEC sp_InsertIndustry @sIndustryName, @sIndustryImage, @dCreateTime, @dUpdateTime", industryNameParam, industryImageParam, createTimeParam, updateTimeParam);
        return true;
    }

    public IEnumerable<ParentCategory> searchParentCategoriesByKeyword(string keyword)
    {
        SqlParameter keywordParam = new SqlParameter("@sKeyword", keyword);
        return _context.ParentCategories.FromSqlRaw("EXEC sp_SearchParentCategoriesByKeyword @sKeyword", keywordParam);
    }

    public bool updateCategory(int categoryID, int industryID, string categoryName, string categoryDesc, string categoryImage)
    {
        SqlParameter categoryIDParam = new SqlParameter("@PK_iCategoryID", categoryID);
        SqlParameter industryIDParam = new SqlParameter("@FK_iParentCategoryID", industryID);
        SqlParameter categoryNameParam = new SqlParameter("@sCategoryName", categoryName);
        SqlParameter categoryDescParam = new SqlParameter("@sCategoryDescription", categoryDesc);
        SqlParameter categryImageParam = new SqlParameter("@sCategoryImage", categoryImage);
        SqlParameter updateTimeParam = new SqlParameter("@dUpdateTime", DateTime.Now);
        _context.Database.ExecuteSqlRaw("EXEC sp_UpdateCategoryByID @PK_iCategoryID, @FK_iParentCategoryID, @sCategoryName, @sCategoryImage, @sCategoryDescription, @dUpdateTime", 
            categoryIDParam,
            industryIDParam,
            categoryNameParam,
            categryImageParam,
            categoryDescParam,
            updateTimeParam
        );
        return true;
    }

    public bool updateIndustry(int industryID, string industryName, string industryImage)
    {
        SqlParameter industryIDParam = new SqlParameter("@PK_iIndustryID", industryID);
        SqlParameter industryNameParam = new SqlParameter("@sIndustryName", industryName);
        SqlParameter industryImageParam = new SqlParameter("@sIndustryImage", industryImage);
        SqlParameter updateTimeParam = new SqlParameter("@dUpdateTime", DateTime.Now);
        _context.Database.ExecuteSqlRaw("EXEC sp_UpdateIndustry @PK_iIndustryID, @sIndustryName, @sIndustryImage, @dUpdateTime", industryIDParam, industryNameParam, industryImageParam, updateTimeParam);
        return true;
    }
}