using System.Reflection;

namespace VFM.Models
{
    public class  SupportUserModel
    {
        public SupportUserModel() { }

        public SupportUserModel(UserModel model)
        {
            login = model.login;
            password = model.password;
            isAdmin = model.isAdmin;
            createF = model.createF;
            deleteF = model.deleteF;
            updateNameF = model.updateNameF;
            downloadF = model.downloadF;
            uploadF = model.uploadF;
        }
        
        public static IEnumerable<SupportUserModel> ConvertToViewModel(IEnumerable<UserModel> models)
        {
            List<SupportUserModel> supportUserModels = new List<SupportUserModel>();
            
            foreach(var element in models)
            {
                supportUserModels.Add(new SupportUserModel(element));
            }

            return supportUserModels;

        }

        public string login { get; set; }
        public string password { get; set; }
        public bool? isAdmin { get; set; } = false;
        public bool? createF { get; set; } = false;
        public bool? deleteF { get; set; } = false;
        public bool? updateNameF { get; set; } = false;
        public bool? downloadF { get; set; } = false;
        public bool? uploadF { get; set; } = false;
    }
}
