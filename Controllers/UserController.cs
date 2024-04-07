using LiteDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using VFM.Controllers.Base;
using VFM.Models;
using VFM.Models.Create;
using VFM.Models.View;

namespace VFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase, IAPIController<UserModel, CUserModel>
    {
        private readonly LiteDatabase db;
        public UserController(LiteDatabase db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var users = VUserModel.ConvertToViewModel(
                        db.GetCollection<UserModel>("user")
                        .FindAll()
                        .ToList()
                    );

                return users.Count == 0 ? NoContent() : Ok(users);
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
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromForm] CUserModel model, bool customPar = false)
        {
            try
            {
                var users = db.GetCollection<UserModel>("user");
                UserModel? user = users.Find(element => element.login == model.login).FirstOrDefault();

                if (user != null) throw new Exception("Аккаунт с таким логином уже существует");

                user = new UserModel(model, customPar);

                var InserResult = users.Insert(user);
                user = users.FindById(InserResult);
                user.ID = InserResult;
                users.Update(user);

                string Url = $"{Request.GetDisplayUrl()}/{user.ID}";

                return Created(Url, user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{ID}")]
        public IActionResult Put([FromForm] CUserModel model, int ID, bool customPar = false)
        {
            try
            {
                var users = db.GetCollection<UserModel>("user");
                UserModel user = users.Find(element => element.ID == ID).FirstOrDefault() 
                    ?? throw new Exception("Такого аккаунта не существует");

                user.UpdateModel(model, customPar);                

                user.UpdateModel(user, customPar);
                users.Update(user);

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{ID}")]
        public IActionResult Delete(int ID)
        {
            try
            {
                bool isDelete = db.GetCollection<UserModel>("user").Delete(ID) == false ? 
                    throw new Exception("Пользователя с таким ID не существует") : true;
                return Ok(ID);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Не реалезованные методы

        [NonAction]
        public IActionResult Post([FromForm] CUserModel model)
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
