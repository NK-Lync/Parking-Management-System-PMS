using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Models;

namespace ParkingManagementSystem.Data
{
    public class ParkingDbContext : DbContext
    {
        public ParkingDbContext(DbContextOptions<ParkingDbContext> options) : base(options)
        {
        }

        // Khai báo các bảng (DbSet) tương ứng với các Model
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<RFIDCard> RFIDCards { get; set; }
        public DbSet<MonthlyCard> MonthlyCards { get; set; }
        public DbSet<ParkingPosition> ParkingPositions { get; set; }
        public DbSet<ParkingSession> ParkingSessions { get; set; }
        public DbSet<ParkingTransaction> ParkingTransactions { get; set; }
        public DbSet<LoginAccount> LoginAccounts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình độ chính xác cho các trường kiểu decimal (tiền tệ)
            // Điều này giúp tránh lỗi khi Migration vào SQL Server

            modelBuilder.Entity<Price>()
                .Property(p => p.BasePrice).HasPrecision(18, 2);
            modelBuilder.Entity<Price>()
                .Property(p => p.PricePerHour).HasPrecision(18, 2);

            modelBuilder.Entity<MonthlyCard>()
                .Property(m => m.Balance).HasPrecision(18, 2);

            modelBuilder.Entity<ParkingSession>()
                .Property(s => s.TotalFee).HasPrecision(18, 2);

            modelBuilder.Entity<ParkingTransaction>()
                .Property(t => t.Amount).HasPrecision(18, 2);

            // Cấu hình các quan hệ nếu cần (Fluent API)
            // Ví dụ: Thiết lập khóa chính cho LoginAccount nếu không dùng kiểu int tự tăng
            modelBuilder.Entity<LoginAccount>()
                .HasKey(l => l.Username);
        }
    }
}