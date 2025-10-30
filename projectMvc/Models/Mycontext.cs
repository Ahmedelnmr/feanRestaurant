using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using projectMvc.Models.models;


namespace projectMvc.Models
{
    public class Mycontext : IdentityDbContext<User>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=RestaurantDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
        public Mycontext(DbContextOptions<Mycontext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ManuItem> ManuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<TableReservation> TableReservations { get; set; }
        public DbSet<DailyMenuItemStats> DailyMenuItemStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // ⚠️ لازم دي تكون أول حاجة

            // Category → Menu Items
            modelBuilder.Entity<Category>()
                .HasMany(c => c.ManuItems)
                .WithOne(mi => mi.Category)
                .HasForeignKey(mi => mi.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Default value
            modelBuilder.Entity<Category>()
                .Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            // User → Orders
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order → Order Items
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ManuItem → OrderItems
            modelBuilder.Entity<ManuItem>()
                .HasMany(mi => mi.OrderItems)
                .WithOne(oi => oi.ManuItem)
                .HasForeignKey(oi => oi.ManuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → Table Reservations
            modelBuilder.Entity<User>()
                .HasMany(u => u.TableReservations)
                .WithOne(tr => tr.User)
                .HasForeignKey(tr => tr.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseModel && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseModel)entry.Entity;
                entity.UpdatedAt = DateTime.Now;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.Now;
                    entity.IsDeleted = false;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
