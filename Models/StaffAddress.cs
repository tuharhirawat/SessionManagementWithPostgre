using System.ComponentModel.DataAnnotations;

namespace ZoomColorLab.Models
{
    public class StaffAddress
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public string Address1 { get; set; } = null!;  // First address field, now explicitly required
        public string? Address2 { get; set; }  // Second address field, optional

        [Required]
        public int StaffId { get; set; }  // Foreign key to StaffReg

        [Required]
        [StringLength(1)]
        [RegularExpression("Y|N", ErrorMessage = "Active must be 'Y' or 'N'.")]
        public string? Active { get; set; }  // Status to indicate if the address is active

        // Navigation properties
        public StaffReg? Staff { get; set; }  // Navigation property for StaffReg

       
    }
}
