using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Models
{
    public class ParkingTransaction
    {
        [Key]
        public int TransactionId { get; set; } // Mã giao dịch

        public int SessionId { get; set; } // Liên kết với lượt gửi tương ứng

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } // Số tiền thanh toán

        public string PaymentMethod { get; set; } // Phương thức (Tiền mặt, Chuyển khoản, Thẻ tháng)

        public DateTime TransactionTime { get; set; } // Thời điểm thanh toán

        public string Status { get; set; } // Trạng thái giao dịch (Thành công/Thất bại)

        [ForeignKey("SessionId")]
        public virtual ParkingSession ParkingSession { get; set; }
    }
}