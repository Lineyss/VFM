using VFM.Models.Users;

namespace VFM.Models.Help
{
    public class DBContextInitializator
    {
        public DBContextInitializator()
        {
        }

        public DBContextInitializator(LiteDbContext db)
        {
            var user = db.user.FirstOrDefault(user => user.login == "admin");

            if(user == null)
            { 
                db.user.Add(new User
                {
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

            db.SaveChanges();
        }
    }
}
