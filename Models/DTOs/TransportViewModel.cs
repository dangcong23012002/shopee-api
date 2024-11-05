public class TransportViewModel
{
    public IEnumerable<ShippingOrder> OrdersWaitPickup { get; set; }
    public IEnumerable<ShippingPicker> OrdersPickingUp { get; set; }
    public IEnumerable<ShippingPicker> OrdersAboutedWarehouse { get; set; }
    public IEnumerable<ShippingPicker> OrdersWaitDelivery { get; set; }
    public IEnumerable<ShippingPicker> ShippingWaitDelivery { get; set; }
    public IEnumerable<ShippingDelivery> OrdersDelivering { get; set; }
    public IEnumerable<ShippingDelivery> OrdersDelivered { get; set; }
    public string HtmlOrdersWaitPickupItem { get; set; }
    public string HtmlOrderPickingUpItem { get; set; }
    public string HtmlOrderAboutedWarehouseItem { get; set; }
    public string HtmlOrdersWaitDeliveryItem { get; set; }
    public string HtmlOrdersDeliveringItem { get; set; }
    public string HtmlOrdersDeliveredItem { get; set; }
    public IEnumerable<SellerInfo> SellerInfos { get; set; }
    public IEnumerable<OrderDetail> OrderDetails { get; set; }
    public IEnumerable<OrderDetail> OrderDetailsPickingUp { get; set; }
    public IEnumerable<OrderDetail> OrderDetailsDelivering { get; set; }
    public IEnumerable<Payment> Payments { get; set; }
    public IEnumerable<ShippingOrder> ShippingOrders { get; set; }
    public IEnumerable<ShippingPicker> ShippingPickers { get; set; }
    public IEnumerable<ShippingDelivery>  ShippingDelivering { get; set; }
    public IEnumerable<ShippingDelivery> ShippingDelivered { get; set; }
    public Status Status { get; set; }
    public IEnumerable<Address> DeliveryAddresses { get; set; }
}