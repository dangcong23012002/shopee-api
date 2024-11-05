public class MakeNotice
{
    public int PK_iMakeNoticeID { get; set; }
    public int FK_iUserID { get; set; }
    public int FK_iSellerID { get; set; }
    public DateTime dTime { get; set; }
}