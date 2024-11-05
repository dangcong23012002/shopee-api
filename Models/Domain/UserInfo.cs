public class UserInfo
{
    public int PK_iUserInfoID { get; set; }
    public int FK_iUserID { get; set; }
    public string sDescription { get; set; }
    public string sUserName { get; set; }
    public string sFullName { get; set; }
    public string sEmail { get; set; }
    public DateTime dDateBirth { get; set; }
    public DateTime dUpdateTime { get; set; }
    public int iGender { get; set; }
    public string sImageProfile { get; set; }
    public int iIsLock { get; set; }
}