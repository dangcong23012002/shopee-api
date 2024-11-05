using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using System.Security.Cryptography;
using Azure;

public class UserResponsitory : IUserResponsitory
{
    private readonly DatabaseContext _context;
    public UserResponsitory(DatabaseContext context)
    {
        _context = context;
    }

    public bool changePasswordByUserID(int userID, string password)
    {
        SqlParameter userIDParam = new SqlParameter("@PK_iUserID", userID);
        SqlParameter passwordParam = new SqlParameter("@sPassword", password);
        _context.Database.ExecuteSqlRaw("EXEC sp_ChangePasswordByUserID @PK_iUserID, @sPassword", userIDParam, passwordParam);
        return true;
    }

    public IEnumerable<User> checkEmailUserIsRegis(string email)
    {
        SqlParameter emailParam = new SqlParameter("@sEmail", email);
        return _context.Users.FromSqlRaw("EXEC sp_CheckEmailUserIsRegis @sEmail", emailParam);
    }

    public IEnumerable<UserInfo> checkUserInfoByUserID(int userID)
    {
        SqlParameter userIDParam = new SqlParameter("@FK_iUserID", userID);
        return _context.UserInfos.FromSqlRaw("sp_CheckUserInfoByUserID @FK_iUserID", userIDParam);
    }

    public IEnumerable<User> checkUserLogin(int userID)
    {
        SqlParameter userIDParam = new SqlParameter("@PK_iUserID", userID);
        return _context.Users.FromSqlRaw("EXEC sp_CheckUserLogin @PK_iUserID", userIDParam).ToList();
    }

    // Phương thức giải mã
    public string decrypt(string encrypted)
    {
        string hash = "cong@gmail.com";
        byte[] data = Convert.FromBase64String(encrypted);
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        TripleDESCryptoServiceProvider tripDES = new TripleDESCryptoServiceProvider();
        tripDES.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
        tripDES.Mode = CipherMode.ECB;
        ICryptoTransform transform = tripDES.CreateDecryptor();
        byte[] result = transform.TransformFinalBlock(data, 0, data.Length);
        return UTF8Encoding.UTF8.GetString(result);
    }

    // Phương thức mã hoá
    public string encrypt(string decryted)
    {
        string hash = "cong@gmail.com";
        byte[] data = UTF8Encoding.UTF8.GetBytes(decryted);
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        TripleDESCryptoServiceProvider tripDES = new TripleDESCryptoServiceProvider();
        tripDES.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
        tripDES.Mode = CipherMode.ECB;
        ICryptoTransform transform = tripDES.CreateEncryptor();
        byte[] result = transform.TransformFinalBlock(data, 0, data.Length);

        return Convert.ToBase64String(result);
    }

    public IEnumerable<User> getPassswordAccountByEmail(string email)
    {
        SqlParameter emailParam = new SqlParameter("@sEmail", email);
        return _context.Users.FromSqlRaw("EXEC sp_GetPasswordAccountByEmail @sEmail", emailParam);
    }

    public IEnumerable<User> getUserIDAccountByEmail(string email)
    {
        SqlParameter emailParam = new SqlParameter("@sEmail", email);
        return _context.Users.FromSqlRaw("EXEC sp_GetUserIDAccountByEmail @sEmail", emailParam);
    }

    public IEnumerable<UserInfo> getUserInfoByID(int userID)
    {
        SqlParameter userIDParam = new SqlParameter("@FK_iUserID", userID);
        return _context.UserInfos.FromSqlRaw("EXEC sp_GetUserInfoByID @FK_iUserID", userIDParam);
    }

    public IEnumerable<UserInfo> getUsersInfo()
    {
        return _context.UserInfos.FromSqlRaw("EXEC sp_GetUsersInfo");
    }

    public bool insertUserInfo(int userID, string fullName, int gender, string birth, string image)
    {
        SqlParameter userIDParam = new SqlParameter("@FK_iUserID", userID);
        SqlParameter fullNameParam = new SqlParameter("@sFullName", fullName);
        SqlParameter genderParam = new SqlParameter("@iGender", gender);
        SqlParameter birthParam = new SqlParameter("@dDateBirth", Convert.ToDateTime(birth));
        SqlParameter updateTimeParam = new SqlParameter("@dUpdateTime", DateTime.Now);
        SqlParameter imageParam = new SqlParameter("@sImageProfile", image);
        SqlParameter isLockParam = new SqlParameter("@iIsLock", 1);
        _context.Database.ExecuteSqlRaw("SET DATEFORMAT dmy EXEC sp_InsertUserInfo @FK_iUserID, @sFullName, @iGender, @dDateBirth, @dUpdateTime, @sImageProfile, @iIsLock", 
        userIDParam,
        fullNameParam,
        genderParam,
        birthParam,
        updateTimeParam,
        imageParam,
        isLockParam);
        return true;
    }

    public IEnumerable<User> login(string email, string password)
    {
        SqlParameter emailParam = new SqlParameter("@sEmail", email);
        SqlParameter passwordParam = new SqlParameter("@sPassword", password);
        return _context.Users.FromSqlRaw("EXEC sp_LoginEmailAndPassword @sEmail, @sPassword", emailParam, passwordParam);
    }

    public bool register(RegistrastionModel user)
    {
        // Phải đặt enctype="multipart/form-data" thì IFromFile mới có giá trị
        SqlParameter roleIdParam = new SqlParameter("@FK_iRoleID", 1);
        SqlParameter nameParam = new SqlParameter("@sUserName", user.sUserName);
        SqlParameter emailParam = new SqlParameter("@sEmail", user.sEmail);
        SqlParameter createTimeParam = new SqlParameter("@dCreateTime", DateTime.Now);
        SqlParameter passwordParam = new SqlParameter("@sPassword", user.sPassword);
        _context.Database.ExecuteSqlRaw(
            "EXEC sp_InsertUser @FK_iRoleID, @sUserName, @sEmail, @dCreateTime, @sPassword", 
            roleIdParam, nameParam, emailParam, createTimeParam, passwordParam
        );
        return true;
    }

    public bool updateUserInfoByID(int userID, string fullName = "", int gender = 0, string birth = "", string image = "")
    {
        SqlParameter userIDParam = new SqlParameter("@FK_iUserID", userID);
        SqlParameter fullNameParam = new SqlParameter("@sFullName", fullName);
        SqlParameter genderParam = new SqlParameter("@iGender", gender);
        SqlParameter birthParam = new SqlParameter("@dDateBirth", Convert.ToDateTime(birth));
        SqlParameter updateTimeParam = new SqlParameter("@dUpdateTime", DateTime.Now);
        SqlParameter imageParam = new SqlParameter("@sImageProfile", image);
        _context.Database.ExecuteSqlRaw(
            "sp_UpdateProfile @FK_iUserID, @sFullName, @dDateBirth, @dUpdateTime, @iGender, @sImageProfile", 
            userIDParam,
            fullNameParam,
            birthParam,
            updateTimeParam,
            genderParam,
            imageParam
        );
        return true;
    }
}