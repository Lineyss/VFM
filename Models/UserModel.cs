using System.Reflection;
using System.Text.RegularExpressions;
using VFM.Models.Create;

using VFM.Services;
namespace VFM.Models
{
    public class UserModel
    {
        public UserModel() { }

        public UserModel(CUserModel model, bool customPars = false)
        {
            CheckValidData(model);

            login = model.login;
            password = HashPassword.Hash(model.password);
            role = model.role;
            createF = model.createF ?? false;
            deleteF = model.deleteF ?? false;
            downloadF = model.downloadF ?? false;
            uploadF = model.uploadF ?? false;

            if(role == (int)ERoleModel.admin && !customPars)
            {
                createF = true;
                deleteF = true;
                downloadF = true;
                uploadF = true;
            }

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

        public void CheckValidData(CUserModel model)
        {
            if (!CheackValidLogin(model.login)) throw new Exception("Не верный формат логина");

            if (!CheackValidPassword(model.password)) throw new Exception("Не верный формат пароля");

            if (!Enum.IsDefined(typeof(ERoleModel), role)) throw new Exception("Такой роли не существует");
        }

        public void CheckValidData(UserModel model)
        {
            if (!CheackValidLogin(model.login)) throw new Exception("Не верный формат логина");

            if (!CheackValidPassword(model.password)) throw new Exception("Не верный формат пароля");

            if (!Enum.IsDefined(typeof(ERoleModel), role)) throw new Exception("Такой роли не существует");
        }

        public UserModel UpdateModel(UserModel model, bool customPars=false)
        {
            CheckValidData(model);

            login = model.login;
            password = HashPassword.Hash(model.password);

            if (role != (int)ERoleModel.admin && role != model.role && !customPars)
            {
                createF = true;
                deleteF = true;
                downloadF = true;
                uploadF = true;
            }
            else
            {
                createF = model.createF;
                deleteF = model.deleteF;
                downloadF = model.downloadF;
                uploadF = model.uploadF;
            }
            
            role = model.role;

            return this;
        }
        public UserModel UpdateModel(CUserModel model, bool customPars = false)
        {
            CheckValidData(model);

            login = model.login;
            password = HashPassword.Hash(model.password);

            if (role != (int)ERoleModel.admin && role != model.role && !customPars)
            {
                createF = true;
                deleteF = true;
                downloadF = true;
                uploadF = true;
            }
            else
            {
                createF = model.createF;
                deleteF = model.deleteF;
                downloadF = model.downloadF;
                uploadF = model.uploadF;
            }

            role = model.role;

            return this;
        }
        public int ID { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public int role { get; set; } = (int)ERoleModel.user;
        public bool? createF { get; set; } = false;
        public bool? deleteF { get; set; } = false;
        public bool? downloadF { get; set; } = false;
        public bool? uploadF { get; set; } = false;


        private const string regexPassword = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{5,30}$";
        private const string regexLogin = "^[a-zA-Z]{3,20}$";
    }
}
