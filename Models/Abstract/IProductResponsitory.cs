using Project.Models.Domain;

public interface IProductResponsitory {
    IEnumerable<Product> getProductsByParentCategoryID(int parentCategoryID);
    IEnumerable<Product> getProductsByCategoryID(int categoryID);
    IEnumerable<Product> getProductsByParentCategoryIDIfRoleAdmin(int parentCategoryID);
    IEnumerable<Product> getProductsByCategoryIDIfRoleAdmin(int categoryID);
    IEnumerable<Product> getProductByID(int productID);
    IEnumerable<Product> getProductsByCategoryIDAndSortIncre(int categoryID);
    IEnumerable<Product> getProductsByCategoryIDAndSortReduce(int categoryID);
    IEnumerable<Product> searchProductByKeyword(string keyword);
    IEnumerable<Product> checkProductInCart(int productID);
    IEnumerable<Product> checkProductInOrder(int productID);
    bool insertProduct(int storeID, int categoryID, int discountID, int transportID, string productName, int quantity, string productDescription, string imageUrl, double price);
    bool updateProduct(int productID, int storeID, int categoryID, int discountID, int transportID, string productName, int quantity, string productDescription, string imageUrl, double price);
    bool deleteProductByID(int productID);
    bool insertProductReviewer(int userID, int productID, int star, string comment, string image);
    // product discount
    IEnumerable<Discount> getDiscounts();
    IEnumerable<TransportPrice> getTransportPrice();
    // Reviewer
    IEnumerable<Reviewer> getReviewerByProductID(int productID);
}