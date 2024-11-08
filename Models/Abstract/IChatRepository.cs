public interface IChatRepository
{
    bool insertMakeNoice(int userID, int sellerID);
    IEnumerable<MakeFriend> getMakeFriendByUserIDAndShopID(int userID, int shopID);
}