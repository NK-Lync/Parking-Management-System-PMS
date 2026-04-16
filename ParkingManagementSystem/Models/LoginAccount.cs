using System.ComponentModel.DataAnnotations;

namespace ParkingManagementSystem.Models
{
    public class LoginAccount
    {
        [Key]
        [StringLength(50)]
        public string Username { get; set; } // Tên đăng nhập

        [Required]
        public string Password { get; set; } // Mật khẩu (nên được mã hóa)

        public string Role { get; set; } // Vai trò (Admin, Staff)

        public bool IsActive { get; set; } // Trạng thái tài khoản (Đang hoạt động hay bị khóa)
    }
}