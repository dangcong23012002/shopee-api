public class OrderDetail
{
    public int PK_iOrderID { get; set; }
    public int PK_iProductID { get; set; }
    public string sImageUrl { get; set; }
    public string sProductName { get; set; }
    public string sStoreName { get; set; }
    public int iQuantity { get; set; }
    public double dUnitPrice { get; set; }
    public double dPerDiscount { get; set; }
    public double dMoney { get; set; }
    public double dTransportPrice { get; set; }
    public int iOrderStatusCode { get; set; }
    public DateTime dDate { get; set; }
}