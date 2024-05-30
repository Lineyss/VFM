using System.ComponentModel.DataAnnotations;
using VFM.Models.Help;

namespace VFM.Models.Users
{
    public class User : UserForm
    {
        [Key]
        public int ID { get; set; }

        public User() : base() { }

        public User(UserForm model)
        {
            login = model.login;
            password = HashPassword.Hash(model.password);

            isAdmin = model.isAdmin;
            createF = model.createF;
            deleteF = model.deleteF;
            updateNameF = model.updateNameF;
            downloadF = model.downloadF;
            uploadF = model.uploadF;
        }

        public void UpdateModel(UserForm model)
        {
            if (password == model.password)
            {
               if (!CheackValidLogin(model.login)) throw new Exception(ErrorModel.NotValidLogin);
            }

            else
            {
                model.CheckValidData();
                password = HashPassword.Hash(model.password);
            }

            login = model.login;

            isAdmin = model.isAdmin ?? isAdmin;
            createF = model.createF ?? createF;
            deleteF = model.deleteF ?? deleteF;
            updateNameF = model.updateNameF ?? updateNameF;
            downloadF = model.downloadF ?? downloadF;
            uploadF = model.uploadF ?? uploadF;
        }
    }
}
