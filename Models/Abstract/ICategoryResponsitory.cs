public interface ICategoryResponsitory
{
    IEnumerable<ParentCategory> searchParentCategoriesByKeyword(string keyword);
    IEnumerable<Category> getCategories();
    IEnumerable<Category> searchCategoriesByKeyword(string keyword);
    IEnumerable<CategoryModel> getAllCategories();
    IEnumerable<CategoryModel> getAllCategoriesByShopID(int shopID);
    IEnumerable<CategoryModel> getCategoriesByIndustryID(int industryID);
    IEnumerable<CategoryModel> getCategoryByID(int categoryID);
    IEnumerable<Industry> getIndustries();
    IEnumerable<Industry> getIndustryByID(int industryID);
    bool insertIndustry(string industryName, string industryImage);
    bool updateIndustry(int industryID, string industryName, string industryImage);
    bool deleteIndustryByID(int industryID);
    bool inserCategory(int industryID, string categoryName, string categoryImage, string categoryDesc);
    bool updateCategory(int categoryID, int industryID, string categoryName, string categoryDesc, string categoryImage);
    bool delelteCategory(int categoryID);
}