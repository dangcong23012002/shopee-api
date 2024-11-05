public class Reviewer
{
    public int PK_iReviewID { get; set; }
    public int FK_iUserID { get; set; }
    public int FK_iProductID { get; set; }
    public string sUserName { get; set; }
    public string sImageProfile { get; set; }
    public string sCategoryName { get; set; }
    public int iStars { get; set; }
    public string sComment { get; set; }
    public DateTime dCreateTime { get; set; }
    public DateTime dUpdateTime { get; set; }
    public string sReviewerImage { get; set; }
}