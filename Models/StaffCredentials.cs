using System.ComponentModel.DataAnnotations;

namespace ZoomColorLab.Models
{
    public class StaffCredentials
    {
        [Key]
        public int CredentialId { get; set; }  // Unique identifier for the credentials

        // Foreign key reference to StaffReg
        [Required]
        public int StaffId { get; set; }  // The ID of the related staff member

        public string UserName { get; set; } = null!;  // Username for the staff member
        public string Password { get; set; } = null!;  // Password for the staff member

        public int Status { get; set; }  // Status of the credentials (e.g., active, inactive)

        [Required]
        [StringLength(1)]
        [RegularExpression("Y|N")]
        public string Active { get; set; }  // Status indicating if the credentials are active

        // Navigation property to StaffReg
        public StaffReg? Staff { get; set; }  // Navigation property to the related StaffReg entity
    }
}
