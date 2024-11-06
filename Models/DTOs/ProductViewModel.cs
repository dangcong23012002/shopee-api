using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Project.Models;

public class ProductViewModel {
    public IEnumerable<Store> Stores { get; set; }
    public IEnumerable<Product> Products {get; set;}
    public IEnumerable<Category> Categories {get; set;}
    public IEnumerable<CartDetail> CartDetails { get; set; }
    public IEnumerable<User> Users { get; set; }
    public IEnumerable<UserInfo> UserInfos { get; set; }
    public IEnumerable<Checkout> Checkouts { get; set; }
    public IEnumerable<Reviewer> Reviewers { get; set; }
    public IEnumerable<Product> Product { get; set; }
    public IEnumerable<Store> Store { get; set; }
    public int UserID { get; set; }
    public int TotalPage { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int CurrentCategoryID { get; set; }
    public int CartCount { get; set; }
    public int RoleID { get; set; }
    public Status Status { get; set; }
}