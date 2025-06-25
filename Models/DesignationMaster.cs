
using System.ComponentModel.DataAnnotations;
using ZoomColorLab.Models;

namespace ZoomColorLab.Models
{
    public class DesignationMaster
    {
        [Key]
        public int DesignationId { get; set; }
        public string? Name { get; set; }

        [Required]
        [StringLength(1)]
        [RegularExpression("Y|N")]
        public string Active { get; set; }


        public ICollection<StaffReg>? Staffs { get; set; }
    }

}
