public class Favorite
{
    public int PK_iFavoriteID { get; set; }
    public int FK_iProductID { get; set; }
    public int FK_iUserID { get; set; }
    public int bFavorite { get; set; }
}