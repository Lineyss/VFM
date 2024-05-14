using System.Reflection;
using System.Text.RegularExpressions;
using VFM.Models.Help;

namespace VFM.Models.Users
{
    public class UserForm : UserAuth
    {

        public UserForm()
        {
            isAdmin ??= false;
            createF ??= false;
            deleteF ??= false;
            updateNameF ??= false;
            downloadF ??= false;
            uploadF ??= false;
        }

        public bool? isAdmin { get; set; } = false;
        public bool? createF { get; set; } = false;
        public bool? deleteF { get; set; } = false;
        public bool? updateNameF { get; set; } = false;
        public bool? downloadF { get; set; } = false;
        public bool? uploadF { get; set; } = false;
    }
}
