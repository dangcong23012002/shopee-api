public class Address
{
    public int PK_iAddressID { get; set; }
    public int FK_iUserID { get; set; }
    public string sFullName { get; set; }
    public string sPhone { get; set; }
    public string sAddress { get; set; }
    public int iDefault { get; set; }
}