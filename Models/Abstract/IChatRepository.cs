public interface IChatRepository
{
    bool insertMakeNoice(int userID, int sellerID);
    IEnumerable<MakeNotice> getMakeNoticeByUserIDAndShopID(int userID, int shopID);
}