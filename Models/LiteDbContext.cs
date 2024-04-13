using LiteDB;
using VFM.Services;

namespace VFM.Models
{
    public class LiteDbContext : LiteDatabase
    {
        public LiteDbContext(string connectionString, BsonMapper mapper = null) : base(connectionString, mapper)
        {
            var user = GetCollection<UserModel>("user");
            if(user.Count() == 0)
                user.Insert(new UserModel
                {
                    ID = 0,
                    login = "admin",
                    password = HashPassword.Hash("admin"),
                    isAdmin = true,
                    createF  = true,
                    deleteF = true,
                    updateNameF = true,
                    downloadF = true,
                    uploadF = true
                });
        }      
    }
}
