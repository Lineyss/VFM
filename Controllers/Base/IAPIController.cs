using Microsoft.AspNetCore.Mvc;

namespace VFM.Controllers.Base
{
    public interface IAPIController<Model, CModel>
    {
        public IActionResult Get();
        public IActionResult Post(CModel model);
        public IActionResult Put(Model model);
        public IActionResult Delete(int ID);
    }
}
