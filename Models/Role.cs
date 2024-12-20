using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User_Managment.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } // نقش (مثل Admin یا User)

        // هر نقش ممکن است به چند کاربر اختصاص داده شود
        public ICollection<User> Users { get; set; }
    }

}
