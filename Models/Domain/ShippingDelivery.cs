public class ShippingDelivery
{
    public int PK_iShippingDeliveryID { get; set; }
    public int FK_iOrderStatusID { get; set; }
    public int FK_iShippingOrderID { get; set; }
    public int FK_iShippingUnitID { get; set; }
    public int FK_iOrderID { get; set; }
    public int FK_iUserID { get; set; }
    public string sBuyerName { get; set; }
    public string sStoreName { get; set; }
    public DateTime dDate { get; set; }
    public double fTotalPrice { get; set; }
    public string sOrderStatusName { get; set; }
    public string sPaymentName { get; set; }
    public DateTime dDeliveryTime { get; set; }
    public string sShippingUnitName { get; set; }
    public string sDeliverName { get; set; }
}