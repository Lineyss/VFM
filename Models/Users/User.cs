using VFM.Models.Help;

namespace VFM.Models.Users
{
    public class User : UserForm
    {
        public int ID { get; set; }

        public User() : base() { }

        public User(UserForm model)
        {
            login = model.login;
            password = HashPassword.Hash(model.password);
        }

        public void UpdateModel(UserForm model)
        {
            if (password == model.password) CheackValidLogin(model.login);
            else model.CheckValidData();

            login = model.login;
            password = HashPassword.Hash(model.password);

            isAdmin = model.isAdmin ?? isAdmin;
            createF = model.createF ?? createF;
            deleteF = model.deleteF ?? deleteF;
            updateNameF = model.updateNameF ?? updateNameF;
            downloadF = model.downloadF ?? downloadF;
            uploadF = model.uploadF ?? uploadF;
        }
    }
}
