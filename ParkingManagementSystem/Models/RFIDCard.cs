using System.ComponentModel.DataAnnotations;

namespace ParkingManagementSystem.Models
{
    public class RFIDCard
    {
        [Key]
        public int CardId { get; set; } // Mã định danh thẻ

        [Required]
        [StringLength(50)]
        public string UID { get; set; } // Mã UID vật lý của thẻ (đọc từ đầu đọc)

        [StringLength(50)]
        public string RfidCode { get; set; } // Mã định danh riêng (nếu có)

        public bool IsLocked { get; set; } // Trạng thái khóa thẻ (true = bị khóa)

        public string Status { get; set; } = "Sẵn sàng"; // Trạng thái (Sẵn sàng, Đang sử dụng, Mất)

        public virtual ICollection<MonthlyCard> MonthlyCards { get; set; }
        public virtual ICollection<ParkingSession> ParkingSessions { get; set; }
    }
}