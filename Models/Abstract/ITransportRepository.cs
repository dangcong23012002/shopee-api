public interface ITransportRepository
{
    public IEnumerable<ShippingOrder> getShippingOrdersWaitPickup();
    public IEnumerable<ShippingPicker> getShippingPickerPickingUp();
    public IEnumerable<OrderDetail> getOrderDetailWaitPickupByOrderID(int orderID);
    public IEnumerable<OrderDetail> getOrderDetailPickingUpByOrderID(int orderID);
    public IEnumerable<OrderDetail> getOrderDetailShippingDeliveryByOrderID(int orderID);
    public IEnumerable<SellerInfo> getSellerInfoByOrderID(int orderID);
    public IEnumerable<Payment> getPaymentsTypeByOrderID(int orderID);
    public IEnumerable<Order> getOrderWaitPickupByShippingOrderID(int shippingOrderID);
    public bool insertShippingPicker(int shippingOrderID, string pickerName, string pickerImage);
    public bool insertShippingDelivery(int shippingOrderID, int userID, int orderStatusID, string deliveryImage, string deliverName);
    public bool updatePickerImage(int shippingPickerID, string pickerImage);
    public bool updateDeliveryImage(int shippingDeliveryID, string deliveryImage);
    public bool confirmOrderAboutPickingUp(int orderID);
    public bool confirmShippingOrderAboutDelivered(int shippingOrderID);
    public bool confirmShippingOrderAboutDeliveredBuyer(int shippingOrderID);
    public bool confirmShippingPickerAboutTaken(int shippingPickerID);
    public bool confirmShippingPickerAboutingWarehouse(int shippingPickerID);
    public bool confirmShippingPickerAboutedWarehouse(int shippingPickerID);
    public bool confirmShippingPickerAboutedWaitDeliveryTake(int shippingOrderID);
    public bool confirmShippingPickerAboutedDeliveryTaken(int shippingOrderID);
    public bool confirmShippingPickerAboutDelivering(int shippingOrderID);
    public bool confirmShippingDeliveryAboutedDelivering(int shippingDeliveryID);
    public bool confirmShippingDeliveryAboutedDeliveredToBuyer(int shippingDeliveryID);
    public bool confirmShippingOrderAboutWaitPickerTake(int shippingOrderID);
}