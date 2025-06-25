using Microsoft.EntityFrameworkCore;
using ZoomColorLab.Models;

namespace ZoomColorLab.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Customer related tables
        public DbSet<CustomerReg> CustomerRegs { get; set; }
        public DbSet<RegionMaster> RegionMasters { get; set; }
        public DbSet<StateMaster> StateMasters { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<PhoneType> PhoneTypes { get; set; }
        public DbSet<CustomerCategory> CustomerCategories { get; set; }

        // Staff related tables
        public DbSet<StaffReg> StaffRegs { get; set; }
        public DbSet<StaffAddress> StaffAddresses { get; set; }
        public DbSet<StaffContact> StaffContacts { get; set; }
        public DbSet<StaffCredentials> StaffCredentials { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Master tables
        public DbSet<DeptMaster> DeptMasters { get; set; }
        public DbSet<DesignationMaster> DesignationMasters { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Region and State Relationship
            modelBuilder.Entity<RegionMaster>()
                .HasOne(r => r.State)
                .WithMany(s => s.Regions)
                .HasForeignKey(r => r.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StateMaster>()
                .Property(s => s.Active)
                .HasDefaultValue("Y");

            // Customer Relationships
            modelBuilder.Entity<CustomerReg>()
                .HasOne(c => c.Address)
                .WithOne(a => a.Customer)
                .HasForeignKey<CustomerAddress>(a => a.CustomerId);

            modelBuilder.Entity<CustomerReg>()
                .HasMany(c => c.Contacts)
                .WithOne(cc => cc.Customer)
                .HasForeignKey(cc => cc.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerContact>()
                .HasOne(cc => cc.PhoneType)
                .WithMany()
                .HasForeignKey(cc => cc.PhoneTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerReg>()
                .HasOne(cc => cc.CustomerCategory)
                .WithMany()
                .HasForeignKey(cc => cc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Staff Relationships
            modelBuilder.Entity<StaffReg>()
                .HasMany(s => s.Addresses)
                .WithOne(a => a.Staff)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StaffReg>()
                .HasMany(s => s.Contacts)
                .WithOne(c => c.Staff)
                .HasForeignKey(c => c.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StaffReg>()
                .HasMany(s => s.Credentials)
                .WithOne(c => c.Staff)
                .HasForeignKey(c => c.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StaffReg>()
                .HasMany(s => s.AuditLogs)
                .WithOne(a => a.Staff)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StaffContact>()
                .HasOne(c => c.PhoneType)
                .WithMany()
                .HasForeignKey(c => c.PhoneTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StaffReg>()
                .HasOne(s => s.Department)
                .WithMany()
                .HasForeignKey(s => s.DeptId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StaffReg>()
                .HasOne(s => s.Designation)
                .WithMany()
                .HasForeignKey(s => s.DesignationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StaffReg>()
                .HasOne(s => s.Branch)
                .WithMany(b => b.Staffs)
                .HasForeignKey(s => s.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Default values
            modelBuilder.Entity<StaffReg>()
                .Property(s => s.Active)
                .HasDefaultValue("Y");

            modelBuilder.Entity<CustomerReg>()
                .Property(c => c.Active)
                .HasDefaultValue("Y");

            // Indexes for performance optimization (optional, based on usage)
            modelBuilder.Entity<StaffReg>()
                .HasIndex(s => s.StaffId)
                .IsUnique();

        }
    }
}

