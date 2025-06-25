using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZoomColorLab.Models;

namespace ZoomColorLab.Models
{
    public class StaffContact
    {
        [Key]
        public int ContactId { get; set; }  // Unique identifier for the contact

        // Foreign key reference to StaffReg
        [Required]
        public int StaffId { get; set; }  // Staff member's ID

        public string? Phone1 { get; set; }  // Primary phone number
        public string? Phone2 { get; set; }  // Secondary phone number (optional)
        public string? Whatsapp { get; set; }  // WhatsApp number (optional)
        public string? Email { get; set; }  // Email address (optional)

        // Foreign key reference to PhoneType
        [Required]
        public int PhoneTypeId { get; set; }  // Type of phone (e.g., mobile, landline)

        [Required]
        [StringLength(1)]
        [RegularExpression("Y|N")]
        public string Active { get; set; }  // Status indicating if the contact is active

        // Navigation properties
        [ForeignKey("StaffId")]
        public StaffReg? Staff { get; set; }  // Navigation to StaffReg entity

        [ForeignKey("PhoneTypeId")]
        public PhoneType? PhoneType { get; set; }  // Navigation to PhoneType entity
    }
}
