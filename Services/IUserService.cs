using User_Managment.Models;

namespace User_Managment.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(string username, string password); // ثبت‌نام کاربر
        Task<string> AuthenticateAsync(string username, string password); // احراز هویت و تولید توکن JWT
        Task<IEnumerable<User>> GetAllUsersAsync(); // دریافت لیست کاربران
        Task<User> GetUserByIdAsync(int userId);
        Task<User> UpdateAsync(int userId, string username, string password);
        Task DeleteAsync(int userId);// دریافت اطلاعات کاربر بر اساس شناسه
        Task AssignRoleToUserAsync(string username, int roleId); // تخصیص نقش به کاربر
    }
}
