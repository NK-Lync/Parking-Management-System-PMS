using System.ComponentModel.DataAnnotations;

namespace ParkingManagementSystem.Models
{
    public class ParkingPosition
    {
        [Key]
        public int PositionId { get; set; } // Mã vị trí đỗ

        [StringLength(10)]
        public string Zone { get; set; } // Khu vực (VD: Khu A, Khu B)

        public bool IsOccupied { get; set; } // Trạng thái: true = đã có xe đỗ

        public string Status { get; set; } // Trạng thái bảo trì hoặc sẵn sàng

        public virtual ICollection<ParkingSession> ParkingSessions { get; set; }
    }
}