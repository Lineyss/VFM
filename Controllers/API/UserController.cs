using LiteDB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using VFM.Controllers.Base;
using VFM.Models;

namespace VFM.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    [UserAuthorization(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme, PropertyName = "isAdmin", PropertyValue = "True")]
    public class UserController : ControllerBase, IAPIController<UserModel, SupportUserModel>
    {
        private readonly LiteDbContext db;
        public UserController(LiteDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var users = db.GetCollection<UserModel>("user") .FindAll();
                return !users.Any() ? NoContent() : Ok(users);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{ID}")]
        public IActionResult Get(int ID)
        {
            try
            {
                UserModel? userModel = db.GetCollection<UserModel>("user").FindById(ID);

                return userModel == null ? NoContent() : Ok(userModel);
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost]
        public IActionResult _Post([FromForm] SupportUserModel model)
        {
            try
            {
                var users = db.GetCollection<UserModel>("user");
                UserModel? user = users.Find(element => element.login == model.login).FirstOrDefault();

                if (user != null) throw new Exception(ErrorModel.LoginIsExist);

                user = new UserModel(model);

                var InserResult = users.Insert(user);
                user = users.FindById(InserResult);
                user.ID = InserResult;
                users.Update(user);

                string Url = $"{Request.GetDisplayUrl()}/{user.ID}";

                return Created(Url, user);
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPut("{ID}")]
        public IActionResult Put([FromForm] SupportUserModel model, int ID, bool customPar = false)
        {
            try
            {
                var users = db.GetCollection<UserModel>("user");
                UserModel user = users.Find(element => element.ID == ID).FirstOrDefault()
                    ?? throw new Exception(ErrorModel.AccountIsNotExist);

                user.UpdateModel(model, customPar);

                user.UpdateModel(user, customPar);
                users.Update(user);

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpDelete("{ID}")]
        public IActionResult Delete(int ID)
        {
            try
            {
                bool isDelete = db.GetCollection<UserModel>("user").Delete(ID) == false ?
                    throw new Exception(ErrorModel.AccountWithThisIDIsNotExist) : true;
                return Ok(ID);
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        // Не реалезованные методы

        [NonAction]
        public IActionResult Post([FromForm] SupportUserModel model)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public IActionResult Put([FromForm] UserModel model)
        {
            throw new NotImplementedException();
        }
    }
}
