using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VFM.Views.Main
{
    public class IndexModel : PageModel
    {
        public bool isAdmin { get; private set; }
        public void OnGet(bool isAdmin)
        {
            this.isAdmin = isAdmin;
        }
    }
}
