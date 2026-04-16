using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Models
{
    public class Price
    {
        [Key]
        public int PriceId { get; set; } // Mã định danh giá

        public int TypeId { get; set; } // Mã loại xe áp dụng giá này

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; } // Giá mở cửa (giá cơ bản)

        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerHour { get; set; } // Giá tính theo mỗi giờ tiếp theo

        public int FreeMinutes { get; set; } // Số phút miễn phí khi mới vào

        [ForeignKey("TypeId")]
        public virtual VehicleType VehicleType { get; set; }
    }
}