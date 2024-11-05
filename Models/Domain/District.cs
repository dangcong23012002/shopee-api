public class District
{
    public int PK_iDistrictID { get; set; }
    public int FK_iCityID { get; set; }
    public string sDistrictName { get; set; }
    public string sCityName { get; set; }
}