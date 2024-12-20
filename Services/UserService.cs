using System.Text;
using System;
using User_Managment.Models;
using User_Managment.Data;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;

namespace User_Managment.Services
{
    public class UserService : IUserService
    {
        private readonly User_ManagmentDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public UserService(User_ManagmentDbContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        // ثبت‌نام کاربر جدید
        public async Task<User> RegisterAsync(string username, string password)
        {
            var user = new User { Username = username }; // ایجاد شیء کاربر جدید
            var hashedPassword = _passwordHasher.HashPassword(user, password); // رمز عبور برای کاربر هش می‌شود
            user.PasswordHash = hashedPassword; // ذخیره هش رمز عبور در شیء کاربر
            _context.Users.Add(user); // کاربر جدید به پایگاه داده اضافه می‌شود
            await _context.SaveChangesAsync();
            return user; // کاربر جدید برگشت داده می‌شود
        }


        
        // آپدیت کاریر
        public async Task<User> UpdateAsync(int userId, string username, string password)
        {
            var user = await _context.Users.FindAsync(userId); // پیدا کردن کاربر با شناسه مشخص
            if (user == null)
            {
                throw new Exception("User not found"); // اگر کاربر پیدا نشد، خطا داده می‌شود
            }

            user.Username = username; // به‌روزرسانی نام کاربری

            if (!string.IsNullOrEmpty(password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, password); // به‌روزرسانی رمز عبور
            }

            _context.Users.Update(user); // به‌روزرسانی اطلاعات کاربر در پایگاه داده
            await _context.SaveChangesAsync(); // ذخیره تغییرات
            return user; // کاربر به‌روزرسانی‌شده برگشت داده می‌شود
        }


        // حذف کاریر
        public async Task DeleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId); // پیدا کردن کاربر با شناسه مشخص
            if (user == null)
            {
                throw new Exception("User not found"); // اگر کاربر پیدا نشد، خطا داده می‌شود
            }

            _context.Users.Remove(user); // حذف کاربر از پایگاه داده
            await _context.SaveChangesAsync(); // ذخیره تغییرات
        }

        // دریافت لیست تمام کاربران
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync(); // تمام کاربران از پایگاه داده خوانده می‌شوند
        }

        // دریافت اطلاعات کاربر بر اساس شناسه
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId); // کاربر با شناسه مشخص پیدا می‌شود
        }

        // تخصیص نقش به کاربر
        public async Task AssignRoleToUserAsync(string username, int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId); // پیدا کردن کاربر بر اساس شناسه
            var user = await _context.Users.SingleOrDefaultAsync(r => r.Username == username); // پیدا کردن نقش بر اساس نام
            if (user == null || role == null)
            {
                throw new Exception("User or role not found"); // در صورت نبود کاربر یا نقش خطا داده می‌شود
            }

            user.RoleId = role.Id; // تخصیص نقش به کاربر
            _context.Users.Update(user); // به‌روزرسانی اطلاعات کاربر
            await _context.SaveChangesAsync(); // ذخیره تغییرات
        }





        // احراز هویت کاربر و تولید توکن JWT
        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null ||
                _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return GenerateJwtToken(user);
        }


        // تولید توکن JWT
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); // کلید برای رمزنگاری توکن
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); // امضای توکن
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // صادرکننده
                audience: _configuration["Jwt:Audience"], // مخاطب
                expires: DateTime.Now.AddHours(1), // زمان انقضا
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token); // توکن به صورت رشته بازگردانده می‌شود
        }
    }
}
