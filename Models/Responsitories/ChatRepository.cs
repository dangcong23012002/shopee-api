using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project.Models;

public class ChatRepository : IChatRepository
{
    private readonly DatabaseContext _context;
    public ChatRepository(DatabaseContext context)
    {
        _context = context;
    }

    public IEnumerable<MakeFriend> getMakeFriendByUserIDAndShopID(int userID, int shopID)
    {
        SqlParameter userIDParam = new SqlParameter("@FK_iUserID", userID);
        SqlParameter shopIDParam = new SqlParameter("@FK_iShopID", shopID);
        return _context.MakeFriends.FromSqlRaw("EXEC sp_GetMakeFriendByUserIDAndShopID @FK_iUserID, @FK_iShopID", userIDParam, shopIDParam);
    }

    public bool insertMakeNoice(int userID, int sellerID)
    {
        SqlParameter userIDParam = new SqlParameter("@FK_iUserID", userID);
        SqlParameter sellerIDParam = new SqlParameter("@FK_iSellerID", sellerID);
        SqlParameter timeParam = new SqlParameter("@dTime", DateTime.Now);
        _context.Database.ExecuteSqlRaw("EXEC sp_InsertMakeNoice @FK_iUserID, @FK_iSellerID, @dTime", userIDParam, sellerIDParam, timeParam);
        return true;
    }
}