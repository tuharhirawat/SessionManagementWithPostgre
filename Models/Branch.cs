//
using System.ComponentModel.DataAnnotations;
using ZoomColorLab.Models;

namespace ZoomColorLab.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }
        public string BranchName { get; set; } = null!;

        [Required]
        [StringLength(1)]
        [RegularExpression("Y|N")]
        public string Active { get; set; }


        public ICollection<CustomerReg>? Customers { get; set; }
        public ICollection<StaffReg>? Staffs { get; set; }
    }

}
