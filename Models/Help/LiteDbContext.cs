using LiteDB;
using VFM.Models.Users;

namespace VFM.Models.Help
{
    public class LiteDbContext : LiteDatabase
    {
        public LiteDbContext(string connectionString, BsonMapper mapper = null) : base(connectionString, mapper)
        {
            var user = GetCollection<User>("user");
            if (user.Count() == 0)
                user.Insert(new User
                {
                    ID = 1,
                    login = "admin",
                    password = HashPassword.Hash("admin"),
                    isAdmin = true,
                    createF = true,
                    deleteF = true,
                    updateNameF = true,
                    downloadF = true,
                    uploadF = true
                });
        }
    }
}
