public interface ISellerResponsitory
{
    IEnumerable<Seller> getSellers();
    IEnumerable<Seller> loginAccount(string phone, string password);
    IEnumerable<Seller> getSellerAccountByID(int sellerID);
    IEnumerable<Seller> getPasswordSellerAccountByPhone(string phone);
    IEnumerable<Seller> checkSellerAccountByIDAndPass(int sellerID, string password);
    IEnumerable<SellerInfo> getSellerInfoBySellerID(int sellerID);
    IEnumerable<SellerInfo> getSellerInfoByPhone(string phone);
    IEnumerable<SellerInfo> getSellerInfoByShippingOrderID(int shippingOrderID);
    IEnumerable<SellerInfo> getSellerInfoByPhoneAndPassword(string phone, string password);
    bool changePasswordSellerAccount(int sellerID, string password);
    bool registerAccountSeller(string phone, string username, string password);
}