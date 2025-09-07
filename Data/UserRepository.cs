using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using OrderingSystem.Models;
using System.Security.Claims;

namespace OrderingSystem.Data
{
    public class UserRepository
    {
        private readonly DbHelper _db;
        public UserRepository(DbHelper db)
        {
            _db = db;
        }

        public bool Validate(string username, string password) {

            var sql = "SELECT Id, Username FROM Users WHERE Username=@uname AND PasswordHash=@pwd";
            var dt = _db.ExecuteDataTable(sql, new SqlParameter("@uname", username), new SqlParameter("@pwd", password));
            if (dt.Rows.Count == 1)
            {
                return true;  
            }

            return false;
        }
    }
}
