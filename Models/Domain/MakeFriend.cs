public class MakeFriend
{
    public int PK_iMakeFriendID { get; set; }
    public int FK_iUserID { get; set; }
    public int FK_iSellerID { get; set; }
    public int FK_iMakeStatusID { get; set; }
    public string sUserName { get; set; }
    public int iMakeStatusCode { get; set; }
    public string sMakeStatusName { get; set; }
    public string sImageProfile { get; set; }
    public DateTime dTime { get; set; }
}