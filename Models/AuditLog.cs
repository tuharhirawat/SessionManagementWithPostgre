
using System.ComponentModel.DataAnnotations;
using ZoomColorLab.Models;

namespace ZoomColorLab.Models
{
    public class AuditLog
    {
        [Key]
        public int AuditId { get; set; } // This is the primary key
        public int StaffId { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string? ChangesMade { get; set; }
        public string? ActionType { get; set; }

        [Required]
        [StringLength(1)]
        [RegularExpression("Y|N")]
        public string Active { get; set; }


        public StaffReg? Staff { get; set; }
    }

}
