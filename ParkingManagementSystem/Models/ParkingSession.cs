using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Models
{
    public class ParkingSession
    {
        [Key]
        public int SessionId { get; set; } // Mã lượt gửi

        public int? CardId { get; set; } // Thẻ được sử dụng (nullable nếu là xe vãng lai)

        public int? VehicleId { get; set; } // Xe thực hiện gửi

        public int? PositionId { get; set; } // Vị trí đỗ được chỉ định

        public DateTime? CheckInTime { get; set; } // Giờ vào

        public DateTime? CheckOutTime { get; set; } // Giờ ra

        public string? LicensePlateIn { get; set; } // Biển số nhận diện lúc vào

        public string? LicensePlateOut { get; set; } // Biển số nhận diện lúc ra

        public string? ImageIn { get; set; } // Đường dẫn ảnh chụp lúc vào

        public string? ImageOut { get; set; } // Đường dẫn ảnh chụp lúc ra

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalFee { get; set; } // Tổng tiền phải thanh toán

        [ForeignKey("CardId")]
        public virtual RFIDCard RFIDCard { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; }

        [ForeignKey("PositionId")]
        public virtual ParkingPosition ParkingPosition { get; set; }

        public virtual ICollection<ParkingTransaction> ParkingTransactions { get; set; }
    }
}