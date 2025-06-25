using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZoomColorLab.Models
{
    public class StaffReg
    {
        [Key]
        public int StaffId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTimeOffset? DOB { get; set; }  // Provided by user (optional)

        [DataType(DataType.Date)]
        public DateTimeOffset? DOJ { get; set; }  // Provided by user (optional)

        public string? Remarks { get; set; }

        [Required]
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow; // Auto-set

        [Required]
        [StringLength(1)]
        [RegularExpression("Y|N")]
        public string Active { get; set; }

        // Foreign Keys
        public int DeptId { get; set; }
        public int DesignationId { get; set; }
        public int BranchId { get; set; }
        public int CategoryId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(DeptId))]
        public DeptMaster Department { get; set; }

        [ForeignKey(nameof(DesignationId))]
        public DesignationMaster Designation { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; }

        // Related Entities
        public ICollection<StaffAddress>? Addresses { get; set; }
        public ICollection<StaffContact>? Contacts { get; set; }
        public ICollection<StaffCredentials>? Credentials { get; set; }
        public ICollection<AuditLog>? AuditLogs { get; set; }
    }
}
