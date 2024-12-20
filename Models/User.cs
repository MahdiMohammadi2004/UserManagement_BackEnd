using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User_Managment.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }




        // کلید خارجی برای نقش
        [ForeignKey("Role")]
        public int RoleId { get; set; } = 1;

        // ارتباط با مدل Role
        public Role Role { get; set; }
    }

}
