
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZoomColorLab.Models;

namespace ZoomColorLab.Models
{
    public class CustomerReg
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(1)]
        [RegularExpression("Y|N")]
        public string Active { get; set; }

        // This is your only FK to CustomerCategory
        public int CategoryId { get; set; }

        // This tells EF Core CategoryId is the FK
        [ForeignKey(nameof(CategoryId))]
        public CustomerCategory CustomerCategory { get; set; }

        public CustomerAddress? Address { get; set; }

        public ICollection<CustomerContact> Contacts { get; set; } = new List<CustomerContact>();
    }
}
