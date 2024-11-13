public interface ICartReponsitory {
    IEnumerable<CartDetail> getCartInfo(int userID);
    IEnumerable<CartDetail> getProductCartByID(int userID, int productID);
    IEnumerable<Cart> checkCartIDExist();
    bool insertCart();
    IEnumerable<Cart> getCartIDByTime();
    bool insertCartDetail(int userID, int productID, int cartID, int quantity, double unitPrice);
    bool changeQuantity(int userID, int productID, int quantity, double money);
    IEnumerable<CartDetail> checkProduct(int userID, int productID);
    bool deleteProductInCart(int productID, int userID);
    IEnumerable<Product> get12ProductsAndSortAsc();
}