public class DataViewModel
{
    public Status Status { get; set; }
    public IEnumerable<UserInfo> UserInfo { get; set; }
    public int RoleID { get; set; }
    public int UserID { get; set; }
    public string Username { get; set; }
}