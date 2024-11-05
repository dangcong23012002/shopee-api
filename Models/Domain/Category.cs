using System.ComponentModel.DataAnnotations;

public class Category {
    public int PK_iCategoryID { get; set; }
    [Required(ErrorMessage = "Tên thể loại không được trống!")]
    public int FK_iParentCategoryID { get; set; }
    public string sCategoryName { get; set; }
    public string sCategoryImage { get; set; }
    [Required(ErrorMessage = "Không được trống trường!")]
    public string sCategoryDescription { get; set; }
    public int iProductCount { get; set; }
}