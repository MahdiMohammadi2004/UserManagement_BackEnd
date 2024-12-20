using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using User_Managment.Models;
using User_Managment.Services;

namespace User_Managment.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        //ثبت نام کاربر
        [Route("Api/Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] LoginAndRegisterRequest request)
        {
            try
            {
                var Alluser = await _userService.GetAllUsersAsync();
                var User = Alluser.FirstOrDefault(n => n.Username == request.Username);
                if (User != null) 
                {
                    return StatusCode(400, new ApiResponse { Message = "کاربر قبلا  ثبت نام کرده است", Data = null });
                }
               
                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return StatusCode(400, new ApiResponse { Message = "لطفا نام کاربری خود را وارد کنید", Data = null });
                }
                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return StatusCode(400, new ApiResponse { Message = "لطفا رمز عبور خود را وارد کنید", Data = null });
                }
                var user = await _userService.RegisterAsync(request.Username, request.Password);
                return StatusCode(200, new ApiResponse { Message = "ثبت نام با موفقیت انجام شد", Data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Register.");
                return StatusCode(500, new ApiResponse { Message = $"خطایی در سرور رخ داد . بعدا تلاش کنید: {ex.Message}", Data = null });
            }
        }


        //ورود کاربر 
        [Route("Api/Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginAndRegisterRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return StatusCode(400, new ApiResponse { Message = "لطفا نام کاربری خود را وارد کنید", Data = null });
                }
                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return StatusCode(400, new ApiResponse { Message = "لطفا رمز عبور خود را وارد کنید", Data = null });
                }
                var token = await _userService.AuthenticateAsync(request.Username, request.Password);
                return StatusCode(200, new ApiResponse { Message = "ورود با موفقیت انجام شد", Data = new { Token = token } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Login.");
                return StatusCode(500, new ApiResponse { Message = $"خطایی در سرور رخ داد . بعدا تلاش کنید: {ex.Message}", Data = null });
            }
        }


        //ویرایش کاربر
        [Route("Api/UpdateUser/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] LoginAndRegisterRequest request)
        {
            try
            {
                var updatedUser = await _userService.UpdateAsync(id, request.Username, request.Password);

                if (updatedUser == null || updatedUser.Id <= 0)
                {
                    return StatusCode(400, new ApiResponse { Message = "کاربر مورد نظر یافت نشد", Data = null });
                }
                return StatusCode(200, new ApiResponse { Message = "ویرایش با موفقیت انجام شد", Data = updatedUser });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in UpdateUser.");
                return StatusCode(500, new ApiResponse { Message = $"خطایی در سرور رخ داد . بعدا تلاش کنید: {ex.Message}", Data = null });
            }
        }


        //لیست همه کاربران 
        [Route("Api/GetAllUser")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return StatusCode(200, new ApiResponse { Message = "نمایش کاربران با موفقیت انجام شد", Data = users });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllUser.");
                return StatusCode(500, new ApiResponse { Message = $"خطایی در سرور رخ داد . بعدا تلاش کنید: {ex.Message}", Data = null });
            }
        }


        //دریافت کاربر با شناسه ID 
        [Route("Api/User/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return StatusCode(400, new ApiResponse { Message = "کاربر مورد نظر یافت نشد", Data = null });
                }
                return StatusCode(200, new ApiResponse { Message = "عملیات با موفقیت انجام شد", Data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetUser.");
                return StatusCode(500, new ApiResponse { Message = $"خطایی در سرور رخ داد . بعدا تلاش کنید: {ex.Message}", Data = null });
            }
        }


        //حدف کاربر
        [Route("Api/{userId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _userService.DeleteAsync(userId);
                return StatusCode(200, new ApiResponse { Message = "عملیات حذف با موفقیت انجام شد", Data = null });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetUser.");
                return StatusCode(500, new ApiResponse { Message = $"خطایی در سرور رخ داد . بعدا تلاش کنید: {ex.Message}", Data = null });
            }
        }

        //تغییر نقش کاربر
        [Route("Api/Assign-Role")]
        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string UserName, int RoleID)
        {
            try
            {
                await _userService.AssignRoleToUserAsync(UserName, RoleID);
                return StatusCode(200, new ApiResponse { Message = "تغییر نقش با موفقیت انجام شد", Data = new { Username = UserName, RoleId = RoleID } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Assign-Role.");
                return StatusCode(500, new ApiResponse { Message = $"خطایی در سرور رخ داد . بعدا تلاش کنید: {ex.Message}", Data = null });
            }
        }
    }








    public class LoginAndRegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class ApiResponse
    {
        public string Message { get; set; }
        public object? Data { get; set; }
    }
}
