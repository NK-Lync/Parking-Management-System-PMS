using System.ComponentModel.DataAnnotations;

namespace ParkingManagementSystem.Models
{
    public class VehicleType
    {
        [Key]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "Tên loại xe không được để trống")]
        [StringLength(50, ErrorMessage = "Tên loại xe không được vượt quá 50 ký tự")]
        [MinLength(2, ErrorMessage = "Tên loại xe phải có ít nhất 2 ký tự")]
        [Display(Name = "Loại xe")]
        public string TypeName { get; set; }

        [Required(ErrorMessage = "Giá tiền không được để trống")]
        [Range(0, 1000000, ErrorMessage = "Giá tiền phải nằm trong khoảng từ 0 đến 1.000.000 VNĐ")]
        [DataType(DataType.Currency)]
        [Display(Name = "Giá mỗi giờ")]
        public decimal PricePerHour { get; set; }

        public virtual ICollection<Vehicle>? Vehicles { get; set; } = new List<Vehicle>();
    }
}