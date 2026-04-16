
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; } // Mã định danh xe

        [Required]
        [StringLength(20)]
        public string LicensePlate { get; set; } // Biển số xe

        public int TypeId { get; set; } // Mã loại xe (Khóa ngoại)

        public string Description { get; set; } = ""; // Mô tả thêm về xe (Màu sắc, nhãn hiệu)

        [ForeignKey("TypeId")]
        public virtual VehicleType VehicleType { get; set; } // Liên kết tới bảng Loại xe

        public virtual ICollection<MonthlyCard> MonthlyCards { get; set; }
        public virtual ICollection<ParkingSession> ParkingSessions { get; set; }
    }
}