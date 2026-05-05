using Microsoft.EntityFrameworkCore;
using AvalWebBackend.Domain.Entities;

namespace AvalWebBackend.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<DailyStatsCache> DailyStatsCache => Set<DailyStatsCache>();
    public DbSet<WeeklyStatsCache> WeeklyStatsCache => Set<WeeklyStatsCache>();
    public DbSet<MonthlyStatsCache> MonthlyStatsCache => Set<MonthlyStatsCache>();
    public DbSet<WeeklyArchives> WeeklyArchives => Set<WeeklyArchives>();
    public DbSet<MonthlyArchives> MonthlyArchives => Set<MonthlyArchives>();
    public DbSet<YearlyArchives> YearlyArchives => Set<YearlyArchives>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Admin ----------
        modelBuilder.Entity<Admin>(e =>
        {
            e.ToTable("Admins");
            e.HasKey(a => a.Id);
            e.Property(a => a.Username).IsRequired();
            e.Property(a => a.Password).IsRequired();
        });

        // ---------- User (no more old service columns) ----------
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(u => u.Id);
            e.Property(u => u.Username).IsRequired();
            e.Property(u => u.Password).IsRequired();
            e.Property(u => u.FullName);
            e.Property(u => u.PhoneNumber);
            e.Property(u => u.SerialNumber);
            e.Property(u => u.RoleKey).HasDefaultValue("USER");
        });

        // ---------- Ticket ----------
        modelBuilder.Entity<Ticket>(e =>
        {
            e.ToTable("Tickets");
            e.HasKey(t => t.Id);
            e.Property(t => t.UserId).IsRequired();
            e.Property(t => t.Status).HasDefaultValue("pending");
            e.HasOne<User>()
             .WithMany()
             .HasForeignKey(t => t.UserId)
             .OnDelete(DeleteBehavior.Cascade);
            e.Property(t => t.File).HasColumnType("TEXT");
        });

        // ---------- Message ----------
        modelBuilder.Entity<Message>(e =>
        {
            e.ToTable("Messages");
            e.HasKey(m => m.Id);
            e.Property(m => m.TicketId).IsRequired();
            e.Property(m => m.MessageText).HasColumnName("Text");
            e.Property(m => m.IsRead).HasDefaultValue(false);
            e.HasOne<Ticket>()
             .WithMany()
             .HasForeignKey(m => m.TicketId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Transaction ----------
        modelBuilder.Entity<Transaction>(e =>
        {
            e.ToTable("Transactions");
            e.HasKey(t => t.Id);
            e.Property(t => t.Type).IsRequired();
            e.Property(t => t.TransactionDate).IsRequired();
            e.HasIndex(t => t.Type).HasDatabaseName("idx_transactions_type");
            e.HasIndex(t => t.TransactionDate).HasDatabaseName("idx_transactions_date");
        });

        // ---------- Service ----------
        modelBuilder.Entity<Service>(e =>
        {
            e.ToTable("Services");
            e.HasKey(s => s.Id);
            e.Property(s => s.UserId).IsRequired();
            e.Property(s => s.ServiceName).HasColumnName("Service");
            e.Property(s => s.Status).HasDefaultValue("درحال-انجام");
            e.Property(s => s.PaymentType).HasDefaultValue("پرداخت-تکی");
            e.HasOne(s => s.User)
             .WithMany()
             .HasForeignKey(s => s.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Stats Cache tables ----------
        modelBuilder.Entity<DailyStatsCache>().ToTable("DailyStatsCache").HasKey(d => d.Id);
        modelBuilder.Entity<WeeklyStatsCache>().ToTable("WeeklyStatsCache").HasKey(w => w.Id);
        modelBuilder.Entity<MonthlyStatsCache>().ToTable("MonthlyStatsCache").HasKey(m => m.Id);
        modelBuilder.Entity<WeeklyArchives>().ToTable("WeeklyArchives").HasKey(w => w.Id);
        modelBuilder.Entity<MonthlyArchives>().ToTable("MonthlyArchives").HasKey(m => m.Id);
        modelBuilder.Entity<YearlyArchives>().ToTable("YearlyArchives").HasKey(y => y.Id);
    }
}