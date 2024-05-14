using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using VFM.Models.Auth;
using VFM.Models.Help;
using VFM.Models.Users;

namespace VFM.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Auth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme, PropertyName = "isAdmin", PropertyValue = "True")]
    public class UserController : ControllerBase
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
                var users = db.GetCollection<User>("user").FindAll();
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
                User? userModel = db.GetCollection<User>("user").FindById(ID);

                return userModel == null ? NoContent() : Ok(userModel);
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost]
        public IActionResult Post([FromForm] UserForm model)
        {
            try
            {
                var users = db.GetCollection<User>("user");
                User? user = users.Find(element => element.login == model.login).FirstOrDefault();

                if (user != null) throw new Exception(ErrorModel.LoginIsExist);

                user = new User(model);

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
        public IActionResult Put([FromForm] UserForm model, int ID)
        {
            try
            {
                var users = db.GetCollection<User>("user");
                User user = users.Find(element => element.ID == ID).FirstOrDefault()
                    ?? throw new Exception(ErrorModel.AccountIsNotExist);

                user.UpdateModel(model);

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
                if (db.GetCollection<User>("user").Delete(ID)) return Ok(ID);

                throw new Exception(ErrorModel.AccountWithThisIDIsNotExist);
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
