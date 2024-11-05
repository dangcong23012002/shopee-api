using Project.Models;

public interface IUserResponsitory
{
    IEnumerable<User> login(string email, string password);
    bool register(RegistrastionModel user);
    bool insertUserInfo(int userID, string fullName, int gender, string birth, string image);
    IEnumerable<User> checkUserLogin(int userID);
    IEnumerable<User> checkEmailUserIsRegis(string email);
    IEnumerable<UserInfo> checkUserInfoByUserID(int userID);
    IEnumerable<UserInfo> getUserInfoByID(int userID);
    IEnumerable<User> getPassswordAccountByEmail(string email);
    IEnumerable<User> getUserIDAccountByEmail(string email);
    IEnumerable<UserInfo> getUsersInfo();
    bool changePasswordByUserID(int userID, string password);
    bool updateUserInfoByID(int userID, string fullName = "", int gender = 0, string birth = "", string image = "");
    string encrypt(string decryted);
    string decrypt(string encrypted);
}