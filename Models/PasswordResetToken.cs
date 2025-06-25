
using System;
using System.ComponentModel.DataAnnotations;

namespace ZoomColorLab.Models
{
    public class PasswordResetToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StaffId { get; set; }

        [Required]
        public string Token { get; set; } = null!;

        public DateTime Expiration { get; set; }

        public StaffReg? Staff { get; set; }
    }
}
