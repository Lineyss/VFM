using System.Reflection;

namespace VFM.Models.Create
{
    public class CUserModel
    {
        public CUserModel(){}

        public CUserModel(UserModel model)
        {
            login = model.login;
            password = model.password;
            createF = model.createF;
            deleteF = model.deleteF;
            downloadF = model.downloadF;
            uploadF = model.uploadF;
        }
        public string login { get; set; }
        public string password { get; set; }
        public int role { get; set; } = (int)ERoleModel.user;
        public bool? createF { get; set; } = false;
        public bool? deleteF { get; set; } = false;
        public bool? downloadF { get; set; } = false;
        public bool? uploadF { get; set; } = false;
    }
}
