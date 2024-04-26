using System.Reflection;
using System.Text.RegularExpressions;

using VFM.Services;
namespace VFM.Models
{
    public class UserModel
    {
        public UserModel() { }

        public UserModel(SupportUserModel model)
        {
            CheckValidData(model);

            login = model.login;
            password = HashPassword.Hash(model.password);
            isAdmin = model.isAdmin ?? false;
            createF = model.createF ?? false;
            deleteF = model.deleteF ?? false;
            updateNameF = model.updateNameF ?? false;
            downloadF = model.downloadF ?? false;
            uploadF = model.uploadF ?? false;

        }
        public bool CheackValidLogin(string login)
        {
            Regex regex = new Regex(regexLogin);
            return regex.IsMatch(login);
        }

        public bool CheackValidPassword(string password)
        {
            Regex regex = new Regex(regexPassword);
            return regex.IsMatch(password);
        }

        public void CheckValidData(SupportUserModel model)
        {
            if (!CheackValidLogin(model.login)) throw new Exception("Не верный формат логина");

            if (!CheackValidPassword(model.password)) throw new Exception("Не верный формат пароля");
        }

        public void CheckValidData(UserModel model)
        {
            if (!CheackValidLogin(model.login)) throw new Exception("Не верный формат логина");

            if (!CheackValidPassword(model.password)) throw new Exception("Не верный формат пароля");

        }

        public UserModel UpdateModel(UserModel model, bool customPars=false)
        {
            CheckValidData(model);

            login = model.login;
            password = HashPassword.Hash(model.password);

            return this;
        }
        public UserModel UpdateModel(SupportUserModel model, bool customPars = false)
        {
            CheckValidData(model);

            login = model.login;
            password = HashPassword.Hash(model.password);

            return this;
        }
        public int ID { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public bool? isAdmin { get; set; } = false;
        public bool? createF { get; set; } = false;
        public bool? deleteF { get; set; } = false;
        public bool? updateNameF { get; set; } = false;
        public bool? downloadF { get; set; } = false;
        public bool? uploadF { get; set; } = false;


        private const string regexPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[._@$!%*?&\/])[A-Za-z\d._@$!%*?&\/]{3,25}$";
        private const string regexLogin = "^[a-zA-Z0-9_]{3,20}$";
    }
}
