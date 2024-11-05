public interface IOrderResponsitory
{
    IEnumerable<Order> totalMoneyProductInCart(int userID);
    IEnumerable<Order> getOrderByID(int userID, int shopID);
    IEnumerable<Order> getOrderByOrderID(int orderID);
    IEnumerable<Order> getOrdersByUserIDWaitSettlement(int userID);
    IEnumerable<Order> getOrderByUserIDTransiting(int userID);
    IEnumerable<Order> getOrderByUserIDWaitDelivery(int userID);
    IEnumerable<Order> getOrderByUserIDDeliverd(int orderID);
    IEnumerable<Order> getOrderWaitSettlementByOrderID(int orderID);
    IEnumerable<Order> getOrderWaitDeliveryByOrderID(int orderID);
    IEnumerable<Order> getOrderWaitSettlementByShopID(int shopID);
    IEnumerable<Order> getOrderWaitPickupByShopID(int shopID);
    IEnumerable<Order> getOrderWaitPickingUpByOrderID(int orderID);
    IEnumerable<Order> getOrderProcessedByShopID(int shopID);
    IEnumerable<OrderDetail> getProductsOrderByUserIDWaitSettlement(int userID);
    IEnumerable<OrderDetail> getProductsOrderByUserIDTransiting(int userID);
    IEnumerable<OrderDetail> getProductsOrderByUserIDDelivered(int userID);
    IEnumerable<OrderDetail> getProductsOrderByUserID(int userID);
    IEnumerable<OrderDetail> getOrderDetailByOrderID(int orderID);
    IEnumerable<OrderDetail> getOrderDetailPickingUpByOrderID(int orderID);
    IEnumerable<OrderDetail> getOrderDetailWaitDeliveyByOrderID(int orderID);
    IEnumerable<OrderDetail> getProductsOrderByUserIDDelivering(int userID);
    bool confirmOrderAboutWaitPickup(int orderID, int userID);
    bool confirmOrderAboutTransiting(int orderID, int userID);
    bool confirmOrderAboutWaitDelivery(int orderID, int userID);
    bool confirmOrderAboutDelivered(int orderID);
    bool confirmOrderAboutReceived(int orderID);
    bool confirmOrderAboutWaitDelivering(int orderID);
    bool inserOrder(int userID, int shopID, double totalPrice, int orderStatusID, int paymentID);
    bool inserOrderDetail(int orderID, int productID, int quantity, double unitPrice, double money);
}