using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingManagementSystem.Models
{
    public class MonthlyCard
    {
        [Key]
        public int MonthlyCardId { get; set; } // Mã đăng ký thẻ tháng

        [Required(ErrorMessage = "Vui lòng nhập thẻ RFID")]
        public int CardId { get; set; } // Liên kết với thẻ vật lý
        [Required(ErrorMessage = "Vui lòng chọn loại xe xe")]
        public int VehicleId { get; set; } // Liên kết với xe đăng ký

        
        [Required(ErrorMessage = "Tên chủ xe không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên tối đa 100 ký tự")]
        [RegularExpression(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỨỬỮỰỲỴÝỶỸửữựỳỵỷỹ\s]+$",
        ErrorMessage = "Tên chỉ được chứa chữ cái và khoảng trắng")]

        public string OwnerName { get; set; } // Tên chủ xe
        [Required(ErrorMessage = "Vui lòng chọn ngày hết hạn")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; } // Ngày hết hạn thẻ tháng

        
        [Required(ErrorMessage = "Vui lòng nhập số dư")]
        [Range(0, 100000000, ErrorMessage = "Số dư phải >= 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } // Số dư trong thẻ (nếu có nạp tiền)

        [ForeignKey("CardId")]
        public virtual RFIDCard RFIDCard { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; }
    }
}