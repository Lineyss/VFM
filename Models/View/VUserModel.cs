namespace VFM.Models.View
{
    public class VUserModel : UserModel
    {
        public VUserModel(UserModel model)
        {
            try
            {
                var valueEnum = (ERoleModel)model.role;
                Role = valueEnum.ToString();
            }
            catch
            {
                throw new Exception("Такой роли не существует");
            }
            login = model.login;
            password = model.password;
            createF = model.createF;
            deleteF = model.deleteF;
            downloadF = model.downloadF;
            uploadF = model.uploadF;
        }

        public static List<VUserModel> ConvertToViewModel(List<UserModel>? models)
        {
            List<VUserModel> vUserModels = new List<VUserModel>();

            if (models == null) return vUserModels;

            foreach (var element in models)
            {
                vUserModels.Add(new VUserModel(element));
            }

            return vUserModels;
        }
        public new string Role { get; set; }
    }
}
