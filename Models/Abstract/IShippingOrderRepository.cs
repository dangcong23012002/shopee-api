public interface IShippingOrderRepository
{
    bool insertShippingOrder(int shippingUnitID, int orderID);
    IEnumerable<ShippingOrder> getShippingOrders();
    IEnumerable<ShippingPicker> getShippingPickers();
    IEnumerable<ShippingPicker> getShippingPickerAboutedWarehouse();
    IEnumerable<ShippingOrder> getShippingOrderByOrderID(int orderID);
    IEnumerable<ShippingOrder> getShippingOrderByID(int shippingOrderID);
    IEnumerable<ShippingPicker> getShippingPickerByOrderID(int orderID);
    IEnumerable<ShippingPicker> getShippingPickerByID(int shippingPickerID);
    IEnumerable<ShippingOrder> getShippingOrderByShopID(int shopID);
    IEnumerable<ShippingDelivery> getShippingDeliveryByDeliverID(int deliverID);
    IEnumerable<ShippingDelivery> getShippingDeliveryByOrderID(int orderID);
    IEnumerable<ShippingDelivery> getShippingDeliveredByOrderID(int orderID);
    IEnumerable<ShippingDelivery> getShippingDeliveryCompleteByDeliverID(int deliverID);
    IEnumerable<ShippingPicker> getShippingPickersAboutWarehouseByOrderID(int orderID);
}
