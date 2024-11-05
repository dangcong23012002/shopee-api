using System.Collections;

/// <summary>
/// Mục đích của interface là để thay thế đa kế thừa lớp của những ngôn ngữ khác (ví dụ như C++, Python…). 
/// Ngoài ra, interface sẽ giúp đồng bộ và thống nhất trong việc phát triển hệ thống trao đổi thông tin.
/// </summary>
public interface IHomeResponsitory {
    IEnumerable<Store> getStores();
    IEnumerable<Product> getProducts();
    IEnumerable<ParentCategory> getParentCategories();
    IEnumerable<Category> getCategoriesByParentCategoryID(int parentCategoryID);
    IEnumerable<Category> getCategories();
    IEnumerable<Product> displayProductsPagination(int pageSize, int pageNumber);
    IEnumerable<Favorite> getFavorites(int userID);
}