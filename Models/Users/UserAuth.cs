using System.Text.RegularExpressions;
using VFM.Models.Help;

namespace VFM.Models.Users
{
    public class UserAuth
    {
        public string login { get; set; }
        public string password { get; set; }

        public void CheckValidData()
        {
            if (!CheackValidLogin(login)) throw new Exception(ErrorModel.NotValidLogin);

            if (!CheackValidPassword(password)) throw new Exception(ErrorModel.NotValidPassword);
        }

        protected bool CheackValidLogin(string login)
        {
            Regex regex = new Regex(regexLogin);
            return regex.IsMatch(login);
        }

        protected bool CheackValidPassword(string password)
        {
            Regex regex = new Regex(regexPassword);
            return regex.IsMatch(password);
        }

        private const string regexPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[._@$!%*?&\/])[A-Za-z\d._@$!%*?&\/]{3,25}$";
        private const string regexLogin = "^[a-zA-Z0-9_]{3,20}$";
    }
}
