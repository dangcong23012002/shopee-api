public class CategoryModel
{
    public int PK_iCategoryID { get; set; }
    public int FK_iParentCategoryID { get; set; }
    public string sParentCategoryName { get; set; }
    public string sCategoryName { get; set; }
    public string sCategoryImage { get; set; }
    public string sCategoryDescription { get; set; }
    public DateTime dCreateTime { get; set; }
    public DateTime dUpdateTime { get; set; }
}