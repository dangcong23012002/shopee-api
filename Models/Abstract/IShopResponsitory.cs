public interface IShopResponsitory
{
    IEnumerable<SliderShop> getSlidersShopByShopID(int shopID);
    IEnumerable<Category> getCategoriesByShopID(int shopID);
    IEnumerable<Product> getProductsByShopID(int shopID);
    IEnumerable<Store> getShopByID(int shopID);
    IEnumerable<Store> getShopByUsername(string shopUsername);
    IEnumerable<Store> getShopByParentCategoryID(int parentCategoryID);
    IEnumerable<Store> getShopByProductID(int productID);
    IEnumerable<Store> getShopBySellerID(int sellerID);
    IEnumerable<Product> getTop3SellingProductsShop(int shopID);
    IEnumerable<Product> getTop10SellingProductsShop(int shopID);
    IEnumerable<Product> getTop10GoodPriceProductsShop(int shopID);
    IEnumerable<Product> getTop10SuggestProductsShop(int shopID);
}