using System.ComponentModel.DataAnnotations;

namespace ParkingManagementSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Họ tên phải từ 3 đến 100 ký tự")]
        // Ràng buộc: Chỉ cho phép chữ cái và khoảng trắng (hỗ trợ Unicode tiếng Việt)
        [RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Họ tên không được chứa số hoặc ký tự đặc biệt")]
        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự")]
        // SỬA LỖI: Chuyển 0-0 thành 0-9 để cho phép nhập số
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Tên đăng nhập không được chứa ký tự đặc biệt")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quyền hạn")]
        [Display(Name = "Quyền hạn")]
        public string Role { get; set; } = "Staff";

        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^(0[3|5|7|8|9])([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng (10 số, VD: 0912345678)")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;
    }
}