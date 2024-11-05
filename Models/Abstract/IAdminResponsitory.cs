public interface IAdminResponsitory
{
    IEnumerable<Order> getOrdersWaitSettlment();
    IEnumerable<Order> getOrderWaitPickup();
    IEnumerable<Order> getOrdersPicking();
    IEnumerable<Order> getOrderDelivering();
    IEnumerable<Order> getOrderCompleted();
}