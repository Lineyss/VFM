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
                var users = db.user.ToList();
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
                User? userModel = db.user.Find(ID);

                return userModel == null ? NoContent() : Ok(userModel);
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] [FromForm] UserForm model)
        {
            try
            {
                User? user = db.user.FirstOrDefault(user => user.login == model.login);

                if (user != null) throw new Exception(ErrorModel.LoginIsExist);

                user = new User(model);

                db.user.Add(user);
                db.SaveChanges();

                string Url = $"{Request.GetDisplayUrl()}/{user.ID}";

                return Created(Url, user);
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPut("{ID}")]
        public IActionResult Put( [FromBody] [FromForm] UserForm model, int ID)
        {
            try
            {
                User user = db.user.Find(ID)
                    ?? throw new Exception(ErrorModel.AccountIsNotExist);

                user.UpdateModel(model);

                db.user.Update(user);
                db.SaveChanges();

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
                User user = db.user.Find(ID)
                    ?? throw new Exception(ErrorModel.AccountWithThisIDIsNotExist);

                db.user.Remove(user);
                db.SaveChanges();

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }
    }
}
